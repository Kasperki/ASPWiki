using Xunit;
using ASPWiki.Model;
using System.Collections.Generic;

namespace ASPWiki.Tests
{
    public class WikiPageTest
    {
        [Fact]
        public void Parent_Should_Return_EmptyString_If_No_Parent()
        {
            WikiPage wikiPage = new WikiPage("This");
            wikiPage.SetPath(null);

            Assert.Equal(string.Empty, wikiPage.Parent);
        }

        [Fact]
        public void Parent_Should_Return_Parent_Title()
        {
            WikiPage W1 = new WikiPage("W1");
            WikiPage W2 = new WikiPage("W2");
            WikiPage W3 = new WikiPage("W3");
            W2.SetPath(W1.Path);
            W3.SetPath(W2.Path);

            Assert.Equal(string.Empty, W1.Parent);
            Assert.Equal("W1", W2.Parent);
            Assert.Equal("W2", W3.Parent);
        }

        [Fact]
        public void SetPath_And_Cotr_Should_set_path()
        {
            WikiPage W1 = new WikiPage("W1");
            WikiPage W2 = new WikiPage("W2");
            WikiPage W3 = new WikiPage("W3");
            W2.SetPath(W1.Path);
            W3.SetPath(W2.Path);

            Assert.Equal(new List<string>() { "W1" }, W1.Path);
            Assert.Equal(new List<string>() { "W1", "W2" }, W2.Path);
            Assert.Equal(new List<string>() { "W1", "W2", "W3" }, W3.Path);
        }

        [Fact]
        public void GetPathString_Should_return_path_in_url()
        {
            WikiPage W1 = new WikiPage("W1");
            WikiPage W2 = new WikiPage("W2");
            WikiPage W3 = new WikiPage("W3");
            W2.SetPath(W1.Path);
            W3.SetPath(W2.Path);

            Assert.Equal("W1", W1.GetPathString());
            Assert.Equal("W1/W2", W2.GetPathString());
            Assert.Equal("W1/W2/W3", W3.GetPathString());
        }

        [Fact]
        public void GetPathToParent_Should_return_path_to_parent_in_url()
        {
            WikiPage W1 = new WikiPage("W1");
            WikiPage W2 = new WikiPage("W2");
            WikiPage W3 = new WikiPage("W3");
            W2.SetPath(W1.Path);
            W3.SetPath(W2.Path);

            Assert.Equal(string.Empty, W1.GetPathToParent());
            Assert.Equal("W1", W2.GetPathToParent());
            Assert.Equal("W1/W2", W3.GetPathToParent());
        }

        [Fact]
        public void GetSizeKiloBytes_Should_Return_Content_Size_In_Kilobytes()
        {
            WikiPage W1 = new WikiPage("W1");
            WikiPage W2 = new WikiPage("W2");
            W2.Content = "THIS IS 0,03 KB";

            Assert.Equal("0 KB", W1.GetSizeKiloBytes());
            Assert.Equal("0,03 KB", W2.GetSizeKiloBytes());
        }

        [Fact]
        public void GetContentSummary_Should_Replace_HeaderTags()
        {
            WikiPage W1 = new WikiPage("W1");
            W1.Content = "<h1>LOL</h1><h2>KEK</h3><h5>LOL</h4>";

            string expected = "<h4>LOL</h4><h4>KEK</h4><h5>LOL</h4>";
            string actual = W1.GetContentSummary();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetContentSummary_Should_Return_Content_After_200_Char_And_Ending_To_ClosinTag()
        {
            string string200 = string.Empty;

            for(int i = 0; i < 200; i++)
            {
                string200 += "a";
            }

            string ending = "<p>WhatIsThis</p>";
            string notExpected = "<p>ThisShouldNotCome</p>";

            WikiPage W1 = new WikiPage("W1");
            W1.Content = string200 + ending + notExpected;

            string actual = W1.GetContentSummary();
            Assert.Equal(string200 + ending, actual);
        }
    }
}
