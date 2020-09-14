﻿using System;
using System.Collections.Generic;
using WebCore.Entities;
using WebCore.Model.Enum;
using WebCore.Model.Entities;

namespace WebCore.Model.Services
{
    public class ModelService
    {
        public static SearchResult SearchDefault(SearchModel model)
        {
            string whereCondition = string.Empty;
            int status = model.Status;
            int timeExpress = model.TimeExpress;
            string startDate = model.StartDate;
            string endDate = model.EndDate;
            string timeZoneLocal = model.TimeZoneLocal;
            //
            string clientTime = Helper.Time.TimeHelper.GetDateByTimeZone(timeZoneLocal);
            //
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                // client time
                DateTime today = Convert.ToDateTime(clientTime);
                if (timeExpress == 1)
                {
                    string strDate = Helper.Time.TimeHelper.FormatToDateSQL(today);
                    DateTime dtime = Convert.ToDateTime(strDate);
                    whereCondition = " AND cast(CreatedDate as Date) = cast('" + dtime + "' as Date)";
                }
                // Yesterday
                if (timeExpress == 2)
                {
                    DateTime dtime = today.AddDays(-1);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date) AND cast(CreatedDate as Date) <= cast('" + today + "' as Date)";
                }
                // ThreeDayAgo
                if (timeExpress == 3)
                {
                    DateTime dtime = today.AddDays(-3);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // SevenDayAgo
                if (timeExpress == 4)
                {
                    DateTime dtime = today.AddDays(-7);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // OneMonthAgo
                if (timeExpress == 5)
                {
                    DateTime dtime = today.AddMonths(-1);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }

                // ThreeMonthAgo
                if (timeExpress == 6)
                {
                    DateTime dtime = today.AddMonths(-3);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // SixMonthAgo
                if (timeExpress == 7)
                {
                    DateTime dtime = today.AddMonths(-6);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // OneYearAgo
                if (timeExpress == 8)
                {
                    DateTime dtime = today.AddYears(-1);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }

                if (status == (int)ModelEnum.Enabled.ENABLED)
                    whereCondition += " AND Enabled = 1 ";
                else if (status == (int)ModelEnum.Enabled.DISABLE)
                    whereCondition += " AND Enabled = 0 ";


                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    DateTime dtime = Convert.ToDateTime(startDate);
                    whereCondition += " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                //
                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (Convert.ToDateTime(endDate) < Convert.ToDateTime(startDate))
                        return new SearchResult()
                        {
                            Status = -1,
                            Message = "Thời gian kết thúc không hợp lệ"
                        };
                    //
                    DateTime dtime = Convert.ToDateTime(endDate);
                    whereCondition += " AND cast(CreatedDate as Date) <= cast('" + dtime + "' as Date)";
                }
                //
                if (status == (int)ModelEnum.Enabled.ENABLED)
                    whereCondition += " AND Enabled = 1 ";
                else if (status == (int)ModelEnum.Enabled.DISABLE)
                    whereCondition += " AND Enabled = 0 ";

                //
                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
        }
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
        public static string DropdownListStatus(int Id)
        {
            try
            {
                var productStatusModels = new List<StatusModel>{
                    new StatusModel(1, "Hiển thị"),
                    new StatusModel(0, "Ẩn")
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
