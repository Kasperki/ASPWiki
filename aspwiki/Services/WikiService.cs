using ASPWiki.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace ASPWiki.Services
{
    public class WikiService : IWikiService
    {
        private readonly IWikiRepository wikiRepository;

        public WikiService(IWikiRepository wikiRepository)
        {
            this.wikiRepository = wikiRepository;
        }

        public List<Node> GetWikiTree(List<WikiPage> wikiPages)
        {
            var wikiTree = new List<Node>();

            foreach (var wikiPage in wikiPages)
            {
                var node = wikiTree.Find(n => n.Name == wikiPage.PathArray[0]);

                if (node == null)
                {
                    node = new Node(wikiPage.PathArray[0]);
                    wikiTree.Add(node);
                }

                if (wikiPage.PathArray.Length == 1)
                    node.WikiPage = wikiPage;
                else
                {
                    BuildTreeNode(node, wikiPage, 1);
                }
            }

            return wikiTree;
        }

        private void BuildTreeNode(Node node, WikiPage wikiPage, int i)
        {
            int index = node.ChildNode.FindIndex(n => n.Name == wikiPage.PathArray[i]);

            if (index == -1)
            {
                node.ChildNode.Add(new Node(wikiPage.PathArray[i]));
                index = 0;
            }

            if (i == wikiPage.PathArray.Length - 1)
                node.ChildNode.Find(n => n.Name == wikiPage.PathArray[i]).WikiPage = wikiPage;
            else
            {
                BuildTreeNode(node.ChildNode[index], wikiPage, ++i);
            }
        }

        //TODO GetByPath is heavy?
        //Should keep repo organized in treeshape by path?
        //CHANGING WIKIPAGE TO ITSELFS CHILD WOULD NOT BE VALID
        //CHANGING WIKIPAGES PATH THAT HAS CHILDS WOULD NOT BE VALID OR WOULD CHANGE childs paths as well?
        public bool IsValidPath(string path, Guid id)
        {
            if (path.IndexOf('/') != -1)
            {
                string parentPath = path.Substring(0, path.LastIndexOf('/'));
                if (wikiRepository.GetByPath(parentPath) == null)
                    throw new Exception("Parent not found");
            }

            WikiPage wikiPage = wikiRepository.GetByPath(path);

            if (wikiPage != null && wikiPage.Id != id)
            {
                throw new Exception("Path already exists");
            }
            else
            {
                return true;
            }
        }

        public void Save(WikiPage wikiPage, IEnumerable<IFormFile> uploads, IIdentity indetity)
        {
            //Path
            wikiPage.SetPath(wikiPage.Path);
            IsValidPath(wikiPage.Path, wikiPage.Id);

            //Version history
            wikiPage.ContentHistory = wikiRepository.GetById(wikiPage.Id)?.ContentHistory ?? new List<string>();

            if (wikiPage.ContentHistory.Count == 0 || !String.Equals(wikiPage.ContentHistory.Last(), wikiPage.Content))
                wikiPage.ContentHistory.Add(wikiPage.Content);

            //Author Name
            if (indetity.Name != null && indetity.Name != String.Empty)
                wikiPage.Author = indetity.Name;

            //Public for not autheticated users
            if (!indetity.IsAuthenticated)
                wikiPage.Public = true;

            //Attachments
            var attachments = BindUploadsToAttacments(uploads, indetity.IsAuthenticated);     
            wikiPage.Attachments = attachments;

            var oldAttachments = wikiRepository.GetById(wikiPage.Id)?.Attachments;

            if (oldAttachments != null)
                wikiPage.Attachments = wikiPage.Attachments.Concat(oldAttachments).ToList();

            //ModifiedTime
            wikiPage.LastModified = DateTime.Now;

            //Persist and save
            wikiRepository.Save(wikiPage);
        }

        private List<Attachment> BindUploadsToAttacments(IEnumerable<IFormFile> uploads, bool isAuthenticated)
        {
            var attachments = new List<Attachment>();

            if (uploads != null && isAuthenticated)
            {
                foreach (var upload in uploads)
                {
                    var attachment = new Attachment
                    {
                        FileId = Guid.NewGuid(),
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        ContentType = upload.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(upload.OpenReadStream()))
                    {
                        attachment.Content = reader.ReadBytes((int)upload.Length);
                    }


                    attachments.Add(attachment);
                }
            }

            return attachments;
        }

        public List<WikiPage> FilterPublic(List<WikiPage> wikiPages)
        {
            return (from entry in wikiPages where entry.Public == true select entry).ToList();
        }

        public void AddVisit(WikiPage wikiPage)
        {
            wikiPage.Visits++;
            //TODO PERSISTS & SAVE TO DATABASE
        }
    }
}
