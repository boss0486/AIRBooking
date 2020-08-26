using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("AttachmentIngredient")]
    public partial class AttachmentIngredient
    {
        public AttachmentIngredient()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string FileID { get; set; }
        public string ForID { get; set; }
        public string CategoryID { get; set; }
        public int TypeID { get; set; }
    }

    // model
    public class AttachmentIngredientCreateModel
    {
        public string Title { get; set; }
        public string FileID { get; set; }
        public string ForID { get; set; }
        public string CategoryID { get; set; }
        public int TypeID { get; set; }
    }
    public class AttachmentIngredientUpdateModel : AttachmentIngredientCreateModel
    {
        public string ID { get; set; }
    }
    public class AttachmentIngredientIDModel
    {
        public string ID { get; set; }
    }
    public class RsAttachmentIngredient
    {

        public string ID { get; set; }
        public string FileID { get; set; }
        public string ForID { get; set; }
        public string CategoryID { get; set; }
        public int TypeID { get; set; }
        public RsAttachmentIngredient(string id, string _fileId, string _forId, string _categoryId, int _typeId)
        {
            ID = id;
            FileID = _fileId;
            ForID = _forId;
            CategoryID = _categoryId;
            TypeID = _typeId;
        }
    }
    public class AttachmentIngredientOption
    {
        public string ID { get; set; }
        public string FileID { get; set; }
        public string ForID { get; set; }
        public string CategoryID { get; set; }
        public int TypeID { get; set; }
    }
}