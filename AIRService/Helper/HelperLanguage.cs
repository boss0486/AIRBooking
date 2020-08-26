using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using QRCoder;
using WebCore.Services;

namespace Helper.Language
{
    public class Resource
    {
        public class MenuText
        {
            public string Account = "Tài khoản";
            public string MenuArea = "Phân vùng";
            public string MenuCategory = "Danh mục menu";
            public string MenuItem = "Menu quản lý";
            public string DataList = "Danh sách";
            public string Details = "Chi tiết";
            public string ChangePassword = "Thay đổi mật khẩu";
            public string Pincode = "Đăng nhập với mã pin";
            public string Setting = "Thiết lập";
        }
        public class Static
        {

        }
        public class Label
        {
            public static string BtnCreate { get; set; } = "Thêm mới";
            public static string BtnUpdate { get; set; } = "Cập nhật";
            public static string BtnDelete { get; set; } = "Xóa";
            public static string BtnSearch { get; set; } = "Tìm kiếm";
            public static string BtnClose { get; set; } = "Đóng";
            public static string BtnOpen { get; set; } = "Mở";
            public static string BtnReset { get; set; } = "Làm mới";
            public static string BtnCancel { get; set; } = "Hủy bỏ";
            public static string HtmlText { get; set; } = "Nội dung";
            public static string Summary { get; set; } = "Mô tả";
            public static string HtmlNote { get; set; } = "Mô tả (html)";
            public static string Active { get; set; } = "Kích hoạt";
            public static string NoActive { get; set; } = "Không kích hoạt";
            public static string IDNo { get; set; } = "IDNo.";
            public static string DataList { get; set; } = "Danh sách";
            public static string Display { get; set; } = "Hiển thị";
            public static string Hide { get; set; } = "Ẩn";
            public static string Category { get; set; } = "Danh mục";
            public static string Status { get; set; } = "Trạng thái";
            public static string State { get; set; } = "Tình trạng";
            public static string Option { get; set; } = "Lựa chọn";
            public static string Photo { get; set; } = "Hình ảnh";
            public static string Title { get; set; } = "Tiêu đề";
            public static string Alias { get; set; } = "Đường dẫn";
            public static string Area { get; set; } = "Phân vùng";
        }
        public class Field
        {

        }
        public class Text
        {

        }
    }
}