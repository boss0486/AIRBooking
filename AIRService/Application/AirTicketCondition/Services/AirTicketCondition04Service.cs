using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using Helper.TimeData;
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
    public interface IAirTicketCondition04Service : IEntityService<Entities.AirTicketCondition04> { }
    public class AirTicketCondition04Service : EntityService<Entities.AirTicketCondition04>, IAirTicketCondition04Service
    {
        public AirTicketCondition04Service() : base() { }
        public AirTicketCondition04Service(System.Data.IDbConnection db) : base(db) { }

        // Condition 04 ##############################################################################################################################################################################################################################################################
        public ActionResult ConditionFee04(AirTicketCondition04ConfigModel model)
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
                if (!Helper.Page.Validate.TestDate(eventStart))
                {
                    return Notifization.Invalid("Thời gian bắt đầu không hợp lệ");
                }
            }
            //
            if (!string.IsNullOrWhiteSpace(eventEnd))
            {
                if (!Helper.Page.Validate.TestDate(eventEnd))
                {
                    return Notifization.Invalid("Thời gian kết thúc không hợp lệ");
                }
            }
            //
            if (TimeFormat.FormatToSQLDate(eventEnd) < TimeFormat.FormatToSQLDate(eventStart))
            {
                return Notifization.Invalid("Thời gian bắt đầu phải <= thời gian kết thúc");
            }

            AirTicketCondition04Service airTicketConditionFeeService = new AirTicketCondition04Service(_connection);
            AirTicketCondition04 airTicketConditionFee = airTicketConditionFeeService.GetAlls(m => m.ConditionID == conditionId.ToLower()).FirstOrDefault();
            if (airTicketConditionFee == null)
            {
                // create 
                airTicketConditionFeeService.Create<string>(new AirTicketCondition04
                {
                    Title = conditionId,
                    ConditionID = conditionId,
                    PlaneNoFrom = planeNoFrom,
                    PlaneNoTo = planeNoTo,
                    TimeStart = TimeFormat.FormatToSQLDate(eventStart),
                    TimeEnd = TimeFormat.FormatToSQLDate(eventEnd),
                    IsApplied = true,
                    Enabled = 1
                });
                return Notifization.Success(MessageText.CreateSuccess);
            }
            // update
            airTicketConditionFee.PlaneNoFrom = planeNoFrom;
            airTicketConditionFee.PlaneNoTo = planeNoTo;
            airTicketConditionFee.TimeStart = TimeFormat.FormatToSQLDate(eventStart);
            airTicketConditionFee.TimeEnd = TimeFormat.FormatToSQLDate(eventEnd);
            airTicketConditionFee.IsApplied = true;
            //
            airTicketConditionFeeService.Update(airTicketConditionFee);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public ActionResult AirTicketCondition04EventEnd(AirTicketCondition04EventEndModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string conditionId = model.ConditionID;
            if (string.IsNullOrWhiteSpace(conditionId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            AirTicketCondition04Service airTicketConditionFeeService = new AirTicketCondition04Service(_connection);
            AirTicketCondition04 airTicketConditionFee = airTicketConditionFeeService.GetAlls(m => m.ConditionID == conditionId.ToLower()).FirstOrDefault();
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

        public AirTicketCondition04 GetAirTicketConditionByConditionID(string conditionId)
        {
            if (string.IsNullOrWhiteSpace(conditionId))
                return new AirTicketCondition04();
            //
            AirTicketCondition04Service airTicketConditionFeeService = new AirTicketCondition04Service(_connection);
            AirTicketCondition04 airTicketConditionFee = airTicketConditionFeeService.GetAlls(m => m.ConditionID == conditionId.ToLower()).FirstOrDefault();
            if (airTicketConditionFee == null)
                return new AirTicketCondition04();
            //
            return airTicketConditionFee;
        }
    }
}
