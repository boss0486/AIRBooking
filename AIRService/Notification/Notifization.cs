using System.Net;
using System.Text;
using System.Web.Mvc;
namespace Notifies.Helper
{
    public static class NotifizationText
    {
        //
        public const string Created = "Thêm mới thành công";
        public const string Updated = "Cập nhật thành công";
        public const string Deleted = "Đã xóa";
        public const string Canceled = "Đã hủy";
        public const string Async_success = "Đã đồng bộ";
        //
        public const string Success = "Ok";
        public const string NotFound = "Không tìm thấy dữ liệu.";
        public const string Invalid = "Dữ liệu không hợp lệ";
        public const string Duplicate = "Dữ liệu đã tồn tại.";
        public const string NotExisted = "Dữ liệu không tồn tại.";
        public const string Forbidden = "Bạn không có quyền truy cập chức năng này.";
        public const string AccessDenied = "Từ chối truy cập";
        public const string Unknown = "Không xác định";
        public const string NotService = "Xin vui lòng truy cập sau";
        public const string UnAuthorized = "Không được phép truy cập";
    }
    public class Notifization
    {
        public static ActionResult Data(string message = "", object data = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DataListModel(message: message, data: data);
        }
        public static ActionResult GetList(string message, object data, object paging)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DataListModel(message: message, data: data, paging: paging);
        }
        //public static ActionResult GetList(string message, object data, object role, object paging)
        //{
        //    InternalNotifization notifization = new InternalNotifization();
        //    return notifization.DataListModel(message: message, data: data, role: role, paging: paging);
        //}
        public static ActionResult ERROR(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.ERROR(message);
        }
        public static ActionResult Invalid(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.INVALID(message);
        }
        public static ActionResult SUCCESS(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.SUCCESS(message);
        }
        public static ActionResult OPTION(string message, object data, int selected = -1)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.OPTION(message, data, selected);
        }
        public static ActionResult OPTION(string message, string data)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.OPTION(message, data);
        }
        public static ActionResult NotFound(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.NOTFOUND(message);
        }

        public static ActionResult NotService
        {
            get
            {
                InternalNotifization notifization = new InternalNotifization();
                return notifization.NotServiceJson;
            }
        }

        public static ActionResult UnAuthorized
        {
            get
            {
                InternalNotifization notifization = new InternalNotifization();
                return notifization.UnAuthorizedJson;
            }
        }
        public static ActionResult DownLoadFile(string message, string path)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DownloadFile(message, path);
        }

        public static ActionResult Data(string message, string data)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DataJson(message, data);

        }
        public static ActionResult TEST(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.TEST(message);
        }
    }

    public class InternalNotifization : Controller
    {
        public ActionResult DataListModel(string message = "", object data = null)
        {
            var jsonResult = Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult DataListModel(string message, object data, object paging)
        {
            var jsonResult = Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
                paging,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //public ActionResult DataListModel(string message, object data, object role, object paging)
        //{
        //    return Json(new
        //    {
        //        status = (int)HttpStatusCode.OK,
        //        message,
        //        data,
        //        role,
        //        paging
        //    });
        //}
        public ActionResult ERROR(string message)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult INVALID(string message)
        {
            return Json(new
            {
                status = 000,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SUCCESS(string message)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OPTION(string message, object data, int selected = -1)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OPTION(string message, string data)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NOTFOUND(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                msg = NotifizationText.NotFound;
            return Json(new
            {
                status = (int)HttpStatusCode.NotFound,
                message = msg,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotServiceJson
        {
            get
            {
                return Json(new
                {
                    status = (int)HttpStatusCode.ServiceUnavailable,
                    message = NotifizationText.NotService,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadFile(string message, string path)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                path
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UnAuthorizedJson
        {
            get
            {
                return Json(new
                {
                    status = (int)HttpStatusCode.Unauthorized,
                    message = NotifizationText.UnAuthorized,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DataJson(string message, string data)
        {

            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult TEST(string message)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message,
            }, JsonRequestBehavior.AllowGet);
        }
    }
    //public class Option
    //{
    //    public static MotifizationOption DATALIST(string message, object data, int selected = 0)
    //    {
    //        return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message, Data = data, Selected = selected };
    //    }
    //    public static MotifizationOption ERRORS(string message)
    //    {
    //        return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = message };
    //    }
    //    public static MotifizationOption SUCCESS(string message)
    //    {
    //        return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message };
    //    }
    //    public static MotifizationOption OPTION(string message, object data, int selected = 0)
    //    {
    //        return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message, Data = data, Selected = selected };
    //    }
    //    public static MotifizationOption NOTFOUND
    //    {
    //        get
    //        {
    //            return new MotifizationOption { Status = (int)HttpStatusCode.NotFound, Message = NotifizationText.NotFound };
    //        }
    //    }
    //    public static MotifizationOption NOTSERVICE
    //    {
    //        get
    //        {
    //            return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = NotifizationText.NotService };
    //        }
    //    }
    //    public static MotifizationOption TEST(string mesage)
    //    {

    //        return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = mesage };

    //    }
    //    // 

    //}
    public class MotifizationModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Paging { get; set; }
    }
    public class ModelMessageAttachment
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Attachment { get; set; }
        public object Paging { get; set; }
    }
    public class MotifizationOption
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public int Selected { get; set; }
    }
    public class ModelMotifizationData
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
