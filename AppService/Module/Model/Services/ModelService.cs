using WebCore.Model.Entities;
using System;
using System.Collections.Generic;
using WebCore.Entities;

namespace WebCore.Model.Services
{
    public class ModelService
    {
        public static string DDLSearchExp(int Id)
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
                    new OptionListModel(1, "Tháng 1"),
                    new OptionListModel(2, "Tháng 2"),
                    new OptionListModel(3, "Tháng 3"),
                    new OptionListModel(4, "Tháng 4"),
                    new OptionListModel(5, "Tháng 5"),
                    new OptionListModel(6, "Tháng 6"),
                    new OptionListModel(7, "Tháng 7"),
                    new OptionListModel(8, "Tháng 8"),
                    new OptionListModel(9, "Tháng 9"),
                    new OptionListModel(10, "Tháng 10"),
                    new OptionListModel(11, "Tháng 11"),
                    new OptionListModel(12, "Tháng 12"),
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
