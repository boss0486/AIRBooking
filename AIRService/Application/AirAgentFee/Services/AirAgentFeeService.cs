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
    public interface IAirAgentFeeService : IEntityService<Entities.AirAgentFee> { }
    public class AirAgentFeeService : EntityService<Entities.AirAgentFee>, IAirAgentFeeService
    {
        public AirAgentFeeService() : base() { }
        public AirAgentFeeService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult AgentFeeConfig(AirAgentFeeConfigModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string agentId = model.AgentID;
            float amount = model.Amount;
            if (amount < 0 && amount > 100000000)
                return Notifization.Invalid("Số tiền giới hạn từ 0 - 100 000 000 đ");
            //
            if (Helper.Current.UserLogin.IsCustomerLogged())
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                agentId = CustomerService.GetCustomerIDByUserID(userId);
            }

            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid(MessageText.Invalid + "2");
            //
            CustomerService customerService = new CustomerService(_connection);
            Customer customer = customerService.GetAlls(m => m.ID == agentId.ToLower() && m.TypeID == CustomerTypeService.GetCustomerTypeIDByType((int)CustomerEnum.CustomerType.AGENT)).FirstOrDefault();
            if (customer == null)
                return Notifization.Invalid(MessageText.Invalid + "3");
            //
            AirAgentFeeService airFeeAgentService = new AirAgentFeeService(_connection);
            var airFeeAgent = airFeeAgentService.GetAlls(m => m.AgentID == agentId).FirstOrDefault();
            if (airFeeAgent == null)
            {
                // create
                airFeeAgentService.Create<string>(new AirAgentFee
                {
                    Title = customer.CodeID,
                    AgentID = agentId,
                    Amount = amount,
                    Enabled = 1
                });
                return Notifization.Success(MessageText.CreateSuccess);
            }
            // update
            airFeeAgent.Amount = amount;
            airFeeAgentService.Update(airFeeAgent);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public ActionResult GetAgentFee(AirAgentFeeModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string agentId = model.AgentID;
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE AgentID = @AgentID";
            AirAgentFeeResult airAirFeeAgent = _connection.Query<AirAgentFeeResult>(sqlQuery, new { AgentID = agentId.ToLower() }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return Notifization.Data("OK", new AirAgentFeeResult
                {
                    AgentID = agentId,
                    Amount = 0
                });
            //
            return Notifization.Data("OK", airAirFeeAgent);
        }
        //##############################################################################################################################################################################################################################################################
        public AirAgentFeeResult GetAgentFeeByCustomerID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE AgentID = @AgentID";
            AirAgentFeeResult airAirFeeAgent = _connection.Query<AirAgentFeeResult>(sqlQuery, new { AgentID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
    }
}
