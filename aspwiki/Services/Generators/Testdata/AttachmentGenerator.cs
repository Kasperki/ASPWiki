using ASPWiki.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPWiki.Services.Generators
{
    public class AttachmentGenerator : GeneratorBase, IGarbageGenerator<Attachment>
    {
        public Attachment Generate()
        {
            Attachment attachment = new Attachment();
            attachment.FileId = Guid.NewGuid();
            attachment.FileName = nouns[random.Next(0, nouns.Count)];

            attachment.Content = ConvertStringToByteArray("CONTENT");
            attachment.ContentType = "text/plain";

            return attachment;
        }

        public List<Attachment> GenerateList(int count)
        {
            List<Attachment> list = new List<Attachment>();

            for (int i = 0; i < count; i++)
            {
                list.Add(Generate());
            }

            return list;
        }

        private byte[] ConvertStringToByteArray(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
