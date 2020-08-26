//
namespace Helper
{
    using System.Net;
    using System.Web.Mvc;

    public static class MessageText
    {
        // for errors
        public const string Duplicate = "Dữ liệu đã tồn tại.";
        public const string Forbidden = "Bạn không có quyền truy cập chức năng này.";
        public const string AccessDenied = "Từ chối truy cập";
        public const string Unknown = "Không xác định";
        public const string NotFound = "Không tìm thấy dữ liệu.";
        public const string Invalid = "Dữ liệu không hợp lệ";
        public const string NotExisted = "Dữ liệu không tồn tại.";
        public const string NotService = "Xin vui lòng truy cập sau";
        public const string Unauthorized = "Không được phép truy cập";
        // for success
        public const string CreateSuccess = "Thêm mới thành công";
        public const string UpdateSuccess = "Cập nhật thành công";
        public const string DeleteSuccess = "Đã xóa";
        public const string CancelSuccess = "Đã hủy";
        public const string AsyncSuccess = "Đã đồng bộ";
        public const string Success = "Ok";
    }
    public class Notifization
    {
        // DATA NOTIFIZATION  ###############################################################################################################################################################
        public static ActionResult Data(string message = null, object data = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DataResult(message: message, data: data);
        }
        public static ActionResult Data(string message = null, object data = null, object paging = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DataResult(message: message, data: data, paging: paging);
        }
        public static ActionResult Data(string message = null, object data = null, object role = null, object paging = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DataResult(message: message, data: data, role: role, paging: paging);
        }

        // ERROR NOTIFIZATION  ###############################################################################################################################################################
        public static ActionResult Error(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.ErrorResult(message);
        }
        public static ActionResult Error(string message, string data)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.ErrorResult(message, data);
        }

        // INVALID NOTIFIZATION  #############################################################################################################################################################
        public static ActionResult Invalid(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.InvalidResult(message);
        }

        // SUCCESS NOTIFIZATION  #############################################################################################################################################################
        public static ActionResult Success(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.SuccessResult(message);
        }
        public static ActionResult Success(string message, string data)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.SuccessResult(message, data);
        }

        // OPTION NOTIFIZATION  #############################################################################################################################################################

        public static ActionResult Option(string message = null, string data = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.OptionResult(message, data);
        }
        public static ActionResult Option(string message = null, object data = null, int selected = -1)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.OptionResult(message, data, selected);
        }

        // NOTFOUND NOTIFIZATION  #############################################################################################################################################################
        public static ActionResult NotFound(string message = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.NotFoundResult(message);
        }

        // STATIC FUNCTION NOTIFIZATION  #####################################################################################################################################################
        public static ActionResult NotService
        {
            get
            {
                InternalNotifization notifization = new InternalNotifization();
                return notifization.NotServiceResult;
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

        // OPTION NOTIFIZATION  #############################################################################################################################################################
        public static ActionResult DownLoadFile(string message = null, string path = null)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.DownloadFile(message, path);
        }

        // TEST NOTIFIZATION  #############################################################################################################################################################
        public static ActionResult TEST(string message)
        {
            InternalNotifization notifization = new InternalNotifization();
            return notifization.TEST(message);
        }

    }

    public class InternalNotifization : Controller
    {
        public ActionResult DataResult(string message = null, object data = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
            });
        }
        public ActionResult DataResult(string message = null, object data = null, object paging = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data,
                paging
            });
        }
        public ActionResult DataResult(string message = null, object data = null, object role = null, object paging = null)
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
        //
        public ActionResult SuccessResult(string message = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SuccessResult(string message, string data)
        {

            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);

        }
        //
        public ActionResult ErrorResult(string message = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ErrorResult(string message, string data)
        {

            return Json(new
            {
                status = (int)HttpStatusCode.ServiceUnavailable,
                message,
                data
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult InvalidResult(string message = null)
        {
            return Json(new
            {
                status = 000,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult OptionResult(string message = null, object data = null, int selected = -1)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OptionResult(string message = null, string data = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                data
            });
        }
        //
        public ActionResult NotFoundResult(string msg = null)
        {
            if (string.IsNullOrEmpty(msg))
                msg = MessageText.NotFound;
            return Json(new
            {
                status = (int)HttpStatusCode.NotFound,
                message = msg,
            }, JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult DownloadFile(string message = null, string path = null)
        {
            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                message,
                path
            }, JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult NotServiceResult
        {
            get
            {
                return Json(new
                {
                    status = (int)HttpStatusCode.ServiceUnavailable,
                    message = MessageText.NotService,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Unauthorized
        {
            get
            {
                return Json(new
                {
                    status = (int)HttpStatusCode.Unauthorized,
                    message = MessageText.Unauthorized,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //
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
        public static MotifizationOption Data(string message = null, object data = null, int selected = 0)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message, Data = data, Selected = selected };
        }
        public static MotifizationOption Error(string message = null)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = message };
        }
        public static MotifizationOption Success(string message = null)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message };
        }
        public static MotifizationOption OptionVal(string message = null, object data = null, int selected = 0)
        {
            return new MotifizationOption { Status = (int)HttpStatusCode.OK, Message = message, Data = data, Selected = selected };
        }
        // static
        public static MotifizationOption NotFound
        {
            get
            {
                return new MotifizationOption { Status = (int)HttpStatusCode.NotFound, Message = MessageText.NotFound };
            }
        }
        public static MotifizationOption NotService
        {
            get
            {
                return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = MessageText.NotService };
            }
        }
        public static MotifizationOption TEST(string mesage)
        {

            return new MotifizationOption { Status = (int)HttpStatusCode.ServiceUnavailable, Message = mesage };

        }
    }
    //public class MotifizationModel
    //{
    //    public int Status { get; set; }
    //    public string Message { get; set; }
    //    public object Data { get; set; }
    //    public object Paging { get; set; }
    //}
    //public class ModelMessageAttachment
    //{
    //    public int Status { get; set; }
    //    public string Message { get; set; }
    //    public object Data { get; set; }
    //    public object Attachment { get; set; }
    //    public object Paging { get; set; }
    //}
    public class MotifizationOption
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public int Selected { get; set; }
    }
    //public class ModelMotifizationData
    //{
    //    public int Status { get; set; }
    //    public string Message { get; set; }
    //    public object Data { get; set; }
    //}
    //public class ModelData
    //{
    //    public object Data { get; set; }
    //}
}
