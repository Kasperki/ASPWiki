using System;
using System.ComponentModel.DataAnnotations;

namespace ASPWiki.Model
{
    public class Attachment
    {
        public Guid FileId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
