
////////namespace Helper
////////{
//////// using CMS.Web.
////////    using Dapper;
////////    using PagedList;
////////    using System;
////////    using System.Linq;
////////    using System.Web;
////////    using USER.Services;

////////    public class Authorization
////////    {
////////        public static int UserGroupID
////////        {
////////            get
////////            {
////////                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
////////                {
////////                    string user = System.Web.HttpContext.Current.User.Identity.Name;
////////                    object o = SqlHelper.ExecuteScalar("SELECT UserGroupID FROM CMS_User WHERE LOWER(UserName) ='" + user.ToLower() + "'");
////////                    if (o != null && !string.IsNullOrEmpty(o.ToString()))
////////                        return Convert.ToInt32(o);
////////                }
////////                return -1;
////////            }
////////        }
////////        public static bool CheckPermissionGroup(int idRoleGroup)
////////        {
////////            try
////////            {
////////                if (Current.IsAdmin)
////////                    return true;

////////                // check group permission for group user
////////                int _userGroupId = UserGroupID;
////////                bool result = false;
////////                using (UserGroupService roleService = new UserGroupService())
////////                {
////////                    result = roleService.UserGroupCheckPermissionGroup(_userGroupId, idRoleGroup);
////////                    if (result)
////////                        return true;
////////                    // check user permission for user
////////                    int _userId = Convert.ToInt32(CMSUtils.GetUserID());
////////                    using (UserService sUser = new UserService())
////////                    {
////////                        result = sUser.UserCheckPermissionGroup(_userId, idRoleGroup);
////////                        if (result)
////////                            return true;
////////                    }
////////                }
////////                return false;
////////            }
////////            catch (Exception)
////////            {
////////                return false;
////////            }
////////        }
////////        public static bool CheckPermissionByGroup(int idRoleGroup, int idRoleId)
////////        {

////////            try
////////            {
////////                if (Current.IsAdmin)
////////                    return true;

////////                /////check group permission for group user
////////                int _userGroupId = UserGroupID;
////////                bool result = false;
////////                using (UserGroupService userGroupService = new UserGroupService())
////////                {
////////                    result = userGroupService.UserGroupCheckPermissionByGroup(_userGroupId, idRoleGroup, idRoleId);
////////                    if (result)
////////                        return true;
////////                    // check user permission for user
////////                    int _userId = Convert.ToInt32(CMSUtils.GetUserID());
////////                    using (UserService sUser = new UserService())
////////                    {
////////                        result = sUser.UserCheckPermissionByGroup(_userId, idRoleGroup, idRoleId);
////////                        if (result)
////////                            return true;
////////                    }
////////                }
////////                return false;
////////            }
////////            catch (Exception)
////////            {
////////                return false;
////////            }
////////        }
////////        public static string CheckPermissionByGroupTest(int idRoleGroup, int idRoleId)
////////        {
////////            return "";
////////            try
////////            {
////////                // check group permission for group user
////////                int _userGroupId = UserGroupID;
////////                bool result = false;
////////                //using (RoleService UGroup = new RoleService())
////////                //{
////////                //    //result = UGroup.UserGroupCheckPermissionByGroup(_userGroupId, idRoleGroup, idRoleId);

////////                //    return UGroup.UserGroupCheckPermissionByGroupTest(_userGroupId, idRoleGroup, idRoleId);


////////                //    if (result)
////////                //        return "ok";
////////                //    // check user permission for user
////////                //    int _userId = Convert.ToInt32(CMSUtils.GetUserID());
////////                //    using (UserService sUser = new UserService())
////////                //    {
////////                //        result = sUser.UserCheckPermissionByGroup(_userId, idRoleGroup, idRoleId);
////////                //        if (result)
////////                //            return "ok";
////////                //    }
////////                //}
////////                return "not" + result;
////////            }
////////            catch 
////////            {
////////                return "not:" + ex;
////////            }
////////        }

////////        public static void NoAccess()
////////        {
////////            HttpContext.Current.Response.Redirect("/manage/not-permission");
////////        }
////////    }

////////}


////////namespace Helper.Roles
////////{


////////    public enum OrderDirect
////////    {
////////        Access = 76,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }


////////    public enum Position
////////    {
////////        Access = 118,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }

////////    public enum Store
////////    {
////////        Access = 43,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }

////////    public enum OrderReport
////////    {
////////        Access = 72,
////////        View = 1
////////    }

////////    public enum BookReport
////////    {
////////        Access = 73,
////////        View = 1
////////    }
////////    public enum ImportReport
////////    {
////////        Access = 107,
////////        View = 1
////////    }
////////    public enum ExportReport
////////    {
////////        Access = 106,
////////        View = 1
////////    }
////////    public enum StoreImport
////////    {
////////        Access = 104,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4
////////    }
////////    public enum StoreExport
////////    {
////////        Access = 103,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4
////////    }
////////    public enum ReportBorrowedReturned
////////    {
////////        Access = 72,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5,
////////        Send = 7

////////    }
////////    public enum ReportBook
////////    {
////////        Access = 73,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }

////////    public enum SchoolEmployee
////////    {
////////        Access = 83,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    public enum Student
////////    {
////////        Access = 82,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    public enum SchoolClass
////////    {
////////        Access = 84,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////    }
////////    public enum SchoolDepartment
////////    {
////////        Access = 87,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    public enum School
////////    {
////////        Access = 86,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4
////////    }
////////    public enum SchoolBookConfig
////////    {
////////        Access = 85,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }


////////    public enum Config
////////    {
////////        Access = 85,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4
////////    }
////////    public enum Teacher
////////    {
////////        Access = 83,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4
////////    }


////////    // 02:  Book category **********
////////    public enum BookCate
////////    {
////////        Access = 2,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    // 19: user **********
////////    public enum User
////////    {
////////        Access = 19,
////////        Block = 0,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5,
////////        Permission = 6

////////    }
////////    // 20: UserGroup **********
////////    public enum UserGroup
////////    {
////////        Access = 20,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5,
////////        Permission = 6
////////    }
////////    // 40: Author **********
////////    public enum Author
////////    {
////////        Access = 40,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    // 42: Publisher **********
////////    public enum Publisher
////////    {
////////        Access = 42,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    // 43: Book **********

////////    public enum Book
////////    {
////////        Access = 43,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    // 56: Suggestion **********

////////    public enum Suggestion
////////    {
////////        Access = 56,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////    // 62: BorrowedReturned **********
////////    public enum BorrowedReturned
////////    {
////////        Access = 62,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }

////////    // 63: OrderBook **********
////////    public enum OrderBook
////////    {
////////        Access = 63,
////////        View = 1,
////////        Create = 2,
////////        Update = 3,
////////        Delete = 4,
////////        Appro = 5
////////    }
////////}


//////////////////INSERT INTO  Permission([PermissionName], [Summary]        , [PermissionGroup], [PermissionVal], [Status]) VALUES   
//////////////////----2	    Phân loại
//////////////////  (N'Xem'         ,N'Phân loại sách'		    ,02               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Phân loại sách'		    ,02               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Phân loại sách'		    ,02               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Phân loại sách'		    ,02               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Phân loại sách'		    ,02               ,5              ,0),
//////////////////----19	    Người dùng
//////////////////  (N'Xem'         , N'Người dùng'			    ,19               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Người dùng'			    ,19               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Người dùng'			    ,19               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Người dùng'			    ,19               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Người dùng'			    ,19               ,5              ,0),

//////////////////----20	    Nhóm quyền
//////////////////  (N'Xem'         ,N'Nhóm quyền'			    ,20               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Nhóm quyền'			    ,20               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Nhóm quyền'			    ,20               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Nhóm quyền'			    ,20               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Nhóm quyền'			    ,20               ,5              ,0),

//////////////////----40	    Tác Giả
//////////////////  (N'Xem'         ,N'Tác Giả'				    ,40               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Tác Giả'				    ,40               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Tác Giả'				    ,40               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Tác Giả'				    ,40               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Tác Giả'				    ,40               ,5              ,0),
//////////////////----42	    Nhà xuất bản
//////////////////  (N'Xem'         ,N'Nhà xuất bản'			,42               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Nhà xuất bản'			,42               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Nhà xuất bản'			,42               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Nhà xuất bản'			,42               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Nhà xuất bản'			,42               ,5              ,0),
//////////////////----43	    Sách
//////////////////  (N'Xem'         ,N'Đầu sách'				,43               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Đầu sách'				,43               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Đầu sách'				,43               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Đầu sách'				,43               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Đầu sách'				,43               ,5              ,0),
//////////////////----56	    Đề xuất đã gửi
//////////////////  (N'Xem'         ,N'Đề xuất'				,56               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Đề xuất'				    ,56               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Đề xuất'				    ,56               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Đề xuất'				    ,56               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Đề xuất'				    ,56               ,5              ,0),
//////////////////----62	    Mượn - trả
//////////////////  (N'Xem'         ,N'Mượn trả'				,62               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Mượn trả'				,62               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Mượn trả'				,62               ,3              ,0),
//////////////////  (N'Xóa'         ,N'Mượn trả'				,62               ,4              ,0),
//////////////////  (N'Duyệt'       ,N'Mượn trả'				,62               ,5              ,1),
//////////////////----63	    Đặt trước
//////////////////  (N'Xem'         , N'Đặt trước'		        ,63               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Đặt trước'				,63               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Đặt trước'				,63               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Đặt trước'				,63               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Đặt trước'				,63               ,5              ,1),
//////////////////----72	    Báo cáo đặt mượn
//////////////////  (N'Xem'         ,N'Báo cáo đặt mượn'        ,72               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Báo cáo đặt mượn'        ,72               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Báo cáo đặt mượn'        ,72               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Báo cáo đặt mượn'        ,72               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Báo cáo đặt mượn'        ,72               ,5              ,0),
//////////////////----73	    Báo cáo sách
//////////////////  (N'Xem'         ,N'Báo cáo sách'			,73               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Báo cáo sách'			,73               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Báo cáo sách'			,73               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Báo cáo sách'			,73               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Báo cáo sách'			,73               ,5              ,0),
//////////////////----76	    Đặt sách
//////////////////  (N'Xem'         ,N'Mượn sách'				,76               ,1              ,0),
//////////////////  (N'Thêm'        ,N'Mượn sách'				,76               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Mượn sách'				,76               ,3              ,0),
//////////////////  (N'Xóa'         ,N'Mượn sách'				,76               ,4              ,0),
//////////////////  (N'Duyệt'       ,N'Mượn sách'				,76               ,5              ,0),
//////////////////----82	    Học Sinh
//////////////////  (N'Xem'         , N'Học sinh'				,82               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Học sinh'				,82               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Học sinh'				,82               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Học sinh'				,82               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Học sinh'				,82               ,5              ,0),
//////////////////----83	    Giáo viên, cán bộ
//////////////////   (N'Xem'         ,N'Giáo viên,cán bộ'		,83               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Giáo viên,cán bộ'		,83               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Giáo viên,cán bộ'		,83               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Giáo viên,cán bộ'		,83               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Giáo viên,cán bộ'		,83               ,5              ,0),
//////////////////----84	    Lớp học
//////////////////  (N'Xem'         ,N'Lớp học'				,84               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Lớp học'				    ,84               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Lớp học'				    ,84               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Lớp học'				    ,84               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Lớp học'				    ,84               ,5              ,0),
//////////////////----85	    Cấu hình mượn sách              
//////////////////  (N'Xem'         ,N'Cấu hình mượn sách'	    ,85               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Cấu hình mượn sách'	    ,85               ,2              ,0),
//////////////////  (N'Sửa'         ,N'Cấu hình mượn sách'	    ,85               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Cấu hình mượn sách'	    ,85               ,4              ,0),
//////////////////  (N'Duyệt'       ,N'Cấu hình mượn sách'	    ,85               ,5              ,0),
//////////////////----86	    Trường học                      
//////////////////  (N'Xem'         ,N'Trường học'			    ,86               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Trường học'			    ,86               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Trường học'			    ,86               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Trường học'			    ,86               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Trường học'			    ,86               ,5              ,0),
//////////////////----87	    Phòng, tổ, bộ môn               
//////////////////  (N'Xem'         ,N'Đơn vị công tác'	    ,87               ,1              ,1),
//////////////////  (N'Thêm'        ,N'Đơn vị công tác'		    ,87               ,2              ,1),
//////////////////  (N'Sửa'         ,N'Đơn vị công tác'		    ,87               ,3              ,1),
//////////////////  (N'Xóa'         ,N'Đơn vị công tác'		    ,87               ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Đơn vị công tác'		    ,87               ,5              ,0),
//////////////////----103	    Xuất kho
//////////////////  (N'Xem'         ,N'Xuất kho'				,103              ,1              ,1),
//////////////////  (N'Thêm'        ,N'Xuất kho'				,103              ,2              ,1),
//////////////////  (N'Sửa'         ,N'Xuất kho'				,103              ,3              ,1),
//////////////////  (N'Xóa'         ,N'Xuất kho'				,103              ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Xuất kho'				,103              ,5              ,0),
//////////////////----104	    Nhập kho
//////////////////  (N'Xem'         ,N'Nhập kho'				,104              ,1              ,1),
//////////////////  (N'Thêm'        ,N'Nhập kho'				,104              ,2              ,1),
//////////////////  (N'Sửa'         ,N'Nhập kho'				,104              ,3              ,1),
//////////////////  (N'Xóa'         ,N'Nhập kho'				,104              ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Nhập kho'				,104              ,5              ,0),
//////////////////----106	    Báo cáo xuất sách
//////////////////  (N'Xem'         ,N'Báo cáo xuất sách'		,106              ,1              ,1),
//////////////////  (N'Thêm'        ,N'Báo cáo xuất sách'		,106              ,2              ,1),
//////////////////  (N'Sửa'         ,N'Báo cáo xuất sách'		,106              ,3              ,1),
//////////////////  (N'Xóa'         ,N'Báo cáo xuất sách'		,106              ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Báo cáo xuất sách'		,106              ,5              ,0),
//////////////////----107	    Báo cáo nhập sách
//////////////////  (N'Xem'         ,N'Báo cáo nhập sách'		,107              ,1              ,1),
//////////////////  (N'Thêm'        ,N'Báo cáo nhập sách'		,107              ,2              ,1),
//////////////////  (N'Sửa'         ,N'Báo cáo nhập sách'		,107              ,3              ,1),
//////////////////  (N'Xóa'         ,N'Báo cáo nhập sách'		,107              ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Báo cáo nhập sách'		,107              ,5              ,0),
//////////////////----118	    Vị trí sách
//////////////////  (N'Xem'         ,N'Vị trí sách'		    ,118              ,1              ,1),
//////////////////  (N'Thêm'        ,N'Vị trí sách'		        ,118              ,2              ,1),
//////////////////  (N'Sửa'         ,N'Vị trí sách'		        ,118              ,3              ,1),
//////////////////  (N'Xóa'         ,N'Vị trí sách'		        ,118              ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Vị trí sách'		        ,118              ,5              ,0),
//////////////////----1124	Phân quyền
//////////////////  (N'Xem'         ,N'Phân quyền'		        ,1124             ,1              ,1),
//////////////////  (N'Thêm'        ,N'Phân quyền'		        ,1124             ,2              ,1),
//////////////////  (N'Sửa'         ,N'Phân quyền'		        ,1124             ,3              ,1),
//////////////////  (N'Xóa'         ,N'Phân quyền'		        ,1124             ,4              ,1),
//////////////////  (N'Duyệt'       ,N'Phân quyền'		        ,1124             ,5              ,0),
//////////////////----1126	Đề xuất nhận được
//////////////////  (N'Xem'         ,N'Đề xuất nhận được'		,1126             ,1             ,0),
//////////////////  (N'Thêm'        ,N'Đề xuất nhận được'		,1126             ,2             ,0),
//////////////////  (N'Sửa'         ,N'Đề xuất nhận được'		,1126             ,3             ,0),
//////////////////  (N'Xóa'         ,N'Đề xuất nhận được'		,1126             ,4             ,0),
//////////////////  (N'Duyệt'       ,N'Đề xuất nhận được'		,1126             ,5             ,1)
//////////////////----select MenuItemID, MenuItemName from View_CONTENT_MenuItem_Joined  WHERE IsPermission = 1 AND Enabled = 1 AND NodeLevel = 3 ORDER BY  MenuItemID