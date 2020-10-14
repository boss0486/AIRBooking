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
    public interface IAirTicketCondition05Service : IEntityService<Entities.AirTicketCondition05> { }
    public class AirTicketCondition05Service : EntityService<Entities.AirTicketCondition05>, IAirTicketCondition05Service
    {
        public AirTicketCondition05Service() : base() { }
        public AirTicketCondition05Service(System.Data.IDbConnection db) : base(db) { }

        // Condition 04 ##############################################################################################################################################################################################################################################################
        public ActionResult ConditionFee05Setting(AirTicketCondition05ConfigModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string flightId = model.FlightLocationID;
            string resbookDesigCode = "";
            if (model.ResBookDesigCode != null || model.ResBookDesigCode.Count() > 0)
                resbookDesigCode = string.Join<string>(",", model.ResBookDesigCode);
            //
            int timePlaceHolder = model.TimePlaceHolder;
            int timeBookHolder = model.TimeBookHolder;
            //
            if (string.IsNullOrWhiteSpace(flightId))
                return Notifization.Invalid("Chặng bay không hợp lệ");
            //
            AirTicketCondition05Service airTicketCondition05Service = new AirTicketCondition05Service(_connection);
            AirTicketCondition05 airTicketCondition05 = airTicketCondition05Service.GetAlls(m => m.FlightLocationID == flightId).FirstOrDefault();
            if (airTicketCondition05 == null)
            {
                // create 
                airTicketCondition05Service.Create<string>(new AirTicketCondition05
                {
                    Title = flightId,
                    FlightLocationID = flightId,
                    ResBookDesigCode = resbookDesigCode,
                    TimePlaceHolder = timePlaceHolder,
                    TimeBookHolder = timeBookHolder,
                    IsApplied = true,
                    Enabled = 1
                });
                return Notifization.Success(MessageText.CreateSuccess);
            }
            // update
            airTicketCondition05.Title = flightId;
            airTicketCondition05.FlightLocationID = flightId;
            airTicketCondition05.ResBookDesigCode = resbookDesigCode;
            airTicketCondition05.TimePlaceHolder = timePlaceHolder;
            airTicketCondition05.TimeBookHolder = timeBookHolder;
            airTicketCondition05.IsApplied = true;
            //
            airTicketCondition05Service.Update(airTicketCondition05);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public ActionResult AirTicketCondition05EventEnd(AirTicketConditionFlightLocationID05Model model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string flightLocationId = model.FlightLocationID;
            if (string.IsNullOrWhiteSpace(flightLocationId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            AirTicketCondition05Service airTicketCondition05Service = new AirTicketCondition05Service(_connection);
            AirTicketCondition05 airTicketCondition05 = airTicketCondition05Service.GetAlls(m => m.FlightLocationID == flightLocationId).FirstOrDefault();
            if (airTicketCondition05 == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            airTicketCondition05.IsApplied = false;
            airTicketCondition05.ResBookDesigCode = null;
            airTicketCondition05.TimePlaceHolder = 0;
            airTicketCondition05.TimeBookHolder = 0;
            //
            airTicketCondition05Service.Update(airTicketCondition05);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public AirTicketCondition05 GetAirTicketConditionByID(AirTicketConditionFlightLocationID05Model model)
        {
            if (model== null) 
                return new AirTicketCondition05();
            //
            string flightId = model.FlightLocationID;
            if (string.IsNullOrWhiteSpace(flightId))
                return new AirTicketCondition05();
            //
            AirTicketCondition05Service airTicketCondition05Service = new AirTicketCondition05Service(_connection);
            AirTicketCondition05 airTicketCondition05 = airTicketCondition05Service.GetAlls(m => m.FlightLocationID == flightId).FirstOrDefault();
            if (airTicketCondition05 == null)
                return new AirTicketCondition05();
            //
            return airTicketCondition05;
        }
    }
}
