using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IAirTicketConditionFeeService : IEntityService<Entities.AirTicketConditionFee> { }
    public class AirTicketConditionFeeService : EntityService<Entities.AirTicketConditionFee>, IAirTicketConditionFeeService
    {
        public AirTicketConditionFeeService() : base() { }
        public AirTicketConditionFeeService(System.Data.IDbConnection db) : base(db) { }

        // Condition 04 ##############################################################################################################################################################################################################################################################
        public ActionResult ConditionFee04(AirTicketConditionFeeConfigModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string conditionId = "04";
            int planeNoFrom = model.PlaneNoFrom;
            int planeNoTo = model.PlaneNoTo;
            string eventStart = model.TimeStart;
            string eventEnd = model.TimeEnd;
            //
            if (planeNoFrom <= 0 && planeNoTo <= 0)
                return Notifization.Invalid("Số hiệu máy bay phải > 0");
            //
            if (planeNoTo <= planeNoFrom)
            {
                return Notifization.Invalid("Số hiệu máy bay bắt đầu phải lớn số hiệu kết thúc");
            }
            //
            if (!string.IsNullOrWhiteSpace(eventStart))
            {
                if (!Helper.Page.Validate.TestDateVN(eventStart))
                {
                    return Notifization.Invalid("Thời gian bắt đầu không hợp lệ");
                }
            }
            //
            if (!string.IsNullOrWhiteSpace(eventEnd))
            {
                if (!Helper.Page.Validate.TestDateVN(eventEnd))
                {
                    return Notifization.Invalid("Thời gian kết thúc không hợp lệ");
                }
            }
            //
            if (Helper.Time.TimeHelper.FormatToDateSQL(eventEnd) < Helper.Time.TimeHelper.FormatToDateSQL(eventStart))
            {
                return Notifization.Invalid("Thời gian bắt đầu phải <= thời gian kết thúc");
            }

            AirTicketConditionFeeService airTicketConditionFeeService = new AirTicketConditionFeeService(_connection);
            AirTicketConditionFee airTicketConditionFee = airTicketConditionFeeService.GetAlls(m => m.ConditionID == conditionId.ToLower()).FirstOrDefault();
            if (airTicketConditionFee == null)
            {
                // create 
                airTicketConditionFeeService.Create<string>(new AirTicketConditionFee
                {
                    Title = conditionId,
                    ConditionID = conditionId,
                    PlaneNoFrom = planeNoFrom,
                    PlaneNoTo = planeNoTo,
                    TimeStart = Helper.Time.TimeHelper.FormatToDateSQL(eventStart),
                    TimeEnd = Helper.Time.TimeHelper.FormatToDateSQL(eventEnd),
                    IsApplied = true,
                    Enabled = 1
                });
                return Notifization.Success(MessageText.CreateSuccess);
            }
            // update
            airTicketConditionFee.PlaneNoFrom = planeNoFrom;
            airTicketConditionFee.PlaneNoTo = planeNoTo;
            airTicketConditionFee.TimeStart = Helper.Time.TimeHelper.FormatToDateSQL(eventStart);
            airTicketConditionFee.TimeEnd = Helper.Time.TimeHelper.FormatToDateSQL(eventEnd);
            airTicketConditionFee.IsApplied = true;
            //
            airTicketConditionFeeService.Update(airTicketConditionFee);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public ActionResult AirTicketConditionEventEnd(AirTicketConditionEventEndModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string conditionId = model.ConditionID;
            if (string.IsNullOrWhiteSpace(conditionId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            AirTicketConditionFeeService airTicketConditionFeeService = new AirTicketConditionFeeService(_connection);
            AirTicketConditionFee airTicketConditionFee = airTicketConditionFeeService.GetAlls(m => m.ConditionID == conditionId.ToLower()).FirstOrDefault();
            if (airTicketConditionFee == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            // update
            airTicketConditionFee.PlaneNoFrom = 0;
            airTicketConditionFee.PlaneNoTo = 0;
            airTicketConditionFee.TimeStart = null;
            airTicketConditionFee.TimeEnd = null;
            airTicketConditionFee.IsApplied = false;
            //
            airTicketConditionFeeService.Update(airTicketConditionFee);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public AirTicketConditionFee GetAirTicketConditionByConditionID(string conditionId)
        {
            if (string.IsNullOrWhiteSpace(conditionId))
                return new AirTicketConditionFee();
            //
            AirTicketConditionFeeService airTicketConditionFeeService = new AirTicketConditionFeeService(_connection);
            AirTicketConditionFee airTicketConditionFee = airTicketConditionFeeService.GetAlls(m => m.ConditionID == conditionId.ToLower()).FirstOrDefault();
            if (airTicketConditionFee == null)
                return new AirTicketConditionFee();
            //
            return airTicketConditionFee;
        }

        // Check Condition ##############################################################################################################################################################################################################################################################
        public static bool CheckTicketCondition(AirTicketConditionCheckModel model)
        {
            bool conditionState = false;
            int planeNo = model.PlaneNo;

            AirTicketConditionFeeService airTicketConditionFeeService = new AirTicketConditionFeeService();
            var airTicketCondition = airTicketConditionFeeService.GetAirTicketConditionByConditionID("04");
            if (airTicketCondition != null)
            {
                if (airTicketCondition.IsApplied)
                {
                    if (planeNo >= airTicketCondition.PlaneNoFrom && planeNo <= airTicketCondition.PlaneNoTo)
                    {
                        if (model.DepartureDateTime != null && airTicketCondition.TimeStart != null && airTicketCondition.TimeEnd != null)
                        {
                            // kiem tra thoi gian di
                            DateTime _departureDateTime = Convert.ToDateTime(model.DepartureDateTime);
                            if (_departureDateTime >= airTicketCondition.TimeStart && _departureDateTime <= airTicketCondition.TimeEnd)
                                conditionState = true;
                        }
                        else
                        {
                            conditionState = true;
                        }
                    }
                }
            }
            return conditionState;
        }

    }
}
