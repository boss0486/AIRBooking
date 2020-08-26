//
namespace Helper
{
    using System.Net;
    using System.Web.Mvc;
    public static class ErrorMessages
    {
        public const string DATA_NOTFOUND = "Không tìm thấy dữ liệu.";
        public const string INVALID_INPUT = "Dữ liệu không hợp lệ.";
        public const string DUPLICATE = "Dữ liệu đã tồn tại.";
        public const string NOT_EXISTED = "Dữ liệu không tồn tại.";
        public const string FORBIDDEN = "Bạn không có quyền truy cập chức năng này.";
        public const string ACCESSDENIED = "Từ chối truy cập";
    }
    public static class NotifizationText
    {
        public const string CREATE_SUCCESS = "Thêm mới thành công";
        public const string UPDATE_SUCCESS = "Cập nhật thành công";
        public const string DELETE_SUCCESS = "Đã xóa";
        public const string CANCEL_SUCCESS = "Đã hủy";

        public const string Async_success = "Đã đồng bộ";



        public const string Success = "Ok";
        public const string NotFound = "Không tìm thấy dữ liệu.";
        public const string Invalid = "Dữ liệu không hợp lệ";
        public const string DUPLICATE = "Dữ liệu đã tồn tại.";
        public const string NOTEXISTED = "Dữ liệu không tồn tại.";
        public const string FORBIDDEN = "Bạn không có quyền truy cập chức năng này.";
        public const string ACCESSDENIED = "Từ chối truy cập";
        public const string UNKNOWN = "Không xác định";
        public const string NOTSERVICE = "Xin vui lòng truy cập sau";
        public const string Unauthorized = "Không được phép truy cập";


    }
    public static class User
    {
        public const string ADMIN_USER_NAME = "admin@hotmail.net";
    }
    public class Notifization
    {
        public static ActionResult DATALIST(string message = null, object data = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DATALIST(message: message, data: data);
        }
        public static ActionResult DATALIST(string message = null, object data = null, object paging = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DATALIST(message: message, data: data, paging: paging);
        }
        public static ActionResult DATALIST(string message = null, object data = null, object role = null, object paging = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DATALIST(message: message, data: data, role: role, paging: paging);
        }
        public static ActionResult ERROR(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.ERROR(message);
        }
        public static ActionResult Invalid(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.INVALID(message);
        }
        public static ActionResult Success(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DATASUCCESS(message);
        }
        public static ActionResult OPTION(string message = null, object data = null, int selected = -1)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.OPTION(message, data, selected);
        }
        public static ActionResult OPTION(string message = null, string data = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.OPTION(message, data);
        }
        public static ActionResult NotFound(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.NOTFOUND(message);
        }

        public static ActionResult NotService
        {
            get
            {
                InternalNotifization notifization = new InternalNotifization();
                return notifization.NOTSERVICE;
            }
        }

        public static ActionResult UnAuthorized
        {
            get
            {
                InternalNotifization notifization = new InternalNotifization();
                return notifization.Unauthorized;
            }
        }
        public static ActionResult DownLoadFile(string message = null, string path = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DownloadFile(message, path);
        }

        public static ActionResult Success(string message, string data)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DATASUCCESS(message, data);

        }
        public static ActionResult DataError(string message, string data)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DATAERROR(message, data);

        }
        public static ActionResult TEST(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.TEST(message);
        }
    }

    public class InternalNotifization : Controller
    {
        public ActionResult DATALIST(string message = null, object data = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
            });
        }
        public ActionResult DATALIST(string message = null, object data = null, object paging = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
                paging
            });
        }

        public ActionResult DATALIST(string message = null, object data = null, object role = null, object paging = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
                role,
                paging
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ERROR(string message = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult INVALID(string message = null)
        {
            return Json(new
            {
                status = 000,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult SUCCESS(string message = null)
        //{
        //    return Json(new
        //    {
        //        status = (int)HttpStatusCode.OK,
        //        message
        //    }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult OPTION(string message = null, object data = null, int selected = -1)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OPTION(string message = null, string data = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            });
        }
        public ActionResult NOTFOUND(string msg = null)
        {
            if (string.IsNullOrEmpty(msg))
                msg = NotifizationText.NotFound;
            return Json(new
            {
                status = (int)HttpStatusCode.NotFound,
                message = msg,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NOTSERVICE
        {
            get
            {
                return Json(new
                {
                    status = (int)HttpStatusCode.ServiceUnavailable,
                    message = NotifizationText.NOTSERVICE,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadFile(string message = null, string path = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                path
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Unauthorized
        {
            get
            {
                return Json(new
                {
                    status = (int)HttpStatusCode.Unauthorized,
                    message = NotifizationText.Unauthorized,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DATASUCCESS(string message, string data = null)
        {

            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DATAERROR(string message, string data)
        {

            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message,
                data
            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult TEST(string message)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message
            }, JsonRequestBehavior.AllowGet);
        }
    }
    public class Option
    {
        public static MotifizationOption DATALIST(string message = null, object data = null, int selected = 0)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message, Data = data, Selected = selected };
        }
        public static MotifizationOption ERRORS(string message = null)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = message };
        }
        public static MotifizationOption SUCCESS(string message = null)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message };
        }
        public static MotifizationOption OPTION(string message = null, object data = null, int selected = 0)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message, Data = data, Selected = selected };
        }
        public static MotifizationOption NOTFOUND
        {
            get
            {
                return new MotifizationOption { Status = (int)HttpStatusCode.NotFound, Message = NotifizationText.NotFound };
            }
        }
        public static MotifizationOption NOTSERVICE
        {
            get
            {
                return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = NotifizationText.NOTSERVICE };
            }
        }
        public static MotifizationOption TEST(string mesage)
        {

            return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = mesage };

        }
        // 

    }
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


    public class ModelData
    {
        public object Data { get; set; }
    }

}
