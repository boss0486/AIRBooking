using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("Attachment")]
    public class Attachment : WEBModel
    {
        public Attachment()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
        public float ContentLength { get; set; }
        public string ContentType { get; set; }
    }

    public class ViewAttachment : WEBModel
    {
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
        public float ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ImagePath
        {
            get => Helper.File.AttachmentFile.GetFile(ID);
            set
            {
                ID = Helper.File.AttachmentFile.GetFile(ID);
            }
        }

    }

    public class AttachmentIDModel 
    {
        public string ID { get; set; }
    }
}