using System;
using System.Collections.Generic;
using WebCore.Entities;
using WebCore.Model.Enum;
using WebCore.Model.Entities;

namespace WebCore.Model.Services
{
    public class ModelService
    {
        public static string DropdownListSearchExpress(int id)
        {
            try
            {
                var searchExpOptions = new List<SearchExpressOption>{
                    new SearchExpressOption(1, "Hôm nay"),       // Today
                    new SearchExpressOption(2, "Hôm qua"),       // Yesterday
                    new SearchExpressOption(3, "3 ngày trước"),  // ThreeDayAgo
                    new SearchExpressOption(4, "7 ngày trước"),  // SevenDayAgo
                    new SearchExpressOption(5, "1 tháng trước"), // OneMonthAgo
                    new SearchExpressOption(6, "3 tháng trước"), // ThreeMonthAgo
                    new SearchExpressOption(7, "6 tháng trước"), // SixMonthAgo
                    new SearchExpressOption(8, "1 năm trước")};  // OneYearAgo
                string result = string.Empty;
                foreach (var item in searchExpOptions)
                {
                    string selected = string.Empty;
                    if (item.ID == id)
                        selected = "selected";
                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        public static string DDLStatus(int Id)
        {
            try
            {
                var productStatusModels = new List<StatusModel>{
                    new StatusModel(1, "Hiển thị"),
                    new StatusModel(2, "Ẩn")
                };
                string result = string.Empty;
                foreach (var item in productStatusModels)
                {
                    string selected = string.Empty;
                    if (item.ID == Id)
                        selected = "selected";
                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string DDLMenuSort(int Id)
        {
            try
            {
                List<OptionListModel> optionListModels = new List<OptionListModel> {
                new OptionListModel(1, "Đầu tiên"),
                new OptionListModel(2, "Cuối cùng")};
                string result = string.Empty;
                foreach (var item in optionListModels)
                {
                    string selected = string.Empty;
                    if (Id != -1 && item.ID == Id)
                        selected = "selected";
                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string DDLMonth(int Id)
        {
            try
            {
                List<OptionListModel> optionListModels = new List<OptionListModel>{
                    new OptionListModel{ ID = 01, Title =  "Tháng 01" },
                    new OptionListModel{ ID = 02, Title =  "Tháng 02" },
                    new OptionListModel{ ID = 03, Title =  "Tháng 03" },
                    new OptionListModel{ ID = 04, Title =  "Tháng 04" },
                    new OptionListModel{ ID = 05, Title =  "Tháng 05" },
                    new OptionListModel{ ID = 06, Title =  "Tháng 06" },
                    new OptionListModel{ ID = 07, Title =  "Tháng 07" },
                    new OptionListModel{ ID = 08, Title =  "Tháng 08" },
                    new OptionListModel{ ID = 09, Title =  "Tháng 09" },
                    new OptionListModel{ ID = 10, Title =  "Tháng 10" },
                    new OptionListModel{ ID = 11, Title =  "Tháng 11" },
                    new OptionListModel{ ID = 12, Title =  "Tháng 12" }
                };
                string result = string.Empty;
                foreach (var item in optionListModels)
                {
                    if (item.ID <= DateTime.Now.Month)
                    {
                        string selected = string.Empty;
                        if (Id != -1 && item.ID == Id)
                            selected = "selected";
                        result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                    }
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
