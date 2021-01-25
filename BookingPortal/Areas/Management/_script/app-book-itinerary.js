var pageIndex = 1;
var URLC = "/Management/AirOrder/Action";
var URLA = "/Management/AirOrder";
var arrFile = [];
//
var AirBookItineraryController = {
    init: function () {
        AirBookItineraryController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        //$('#btnSearch').off('click').on('click', function () {
        //    AirBookItineraryController.DataList(1);
        //});
        $('#btnSearch').off('click').on('click', function () {
            AirBookItineraryController.DataList(1);
        });
    },
    DataList: function (page) {
        //   
        var ddlItinerary = $('#ddlItinerary').val();
        var ddlAgentID = $('#ddlAgentID').val();
        var ddlCustomerType = $('#ddlCustomerType').val();
        var ddlCompanyID = $('#ddlCompanyID').val();
        var ddlTimeExpress = $('#ddlTimeExpress').val();
        var txtStartDate = $('#txtStartDate').val();
        var txtEndDate = $('#txtEndDate').val();
        //
        if (ddlCustomerType == "") {
            ddlCustomerType = 0;
        }
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            ItineraryType: parseInt(ddlItinerary),
            AgentID: ddlAgentID,
            CustomerType: ddlCustomerType,
            CompanyID: ddlCompanyID
        };
        //
        AjaxFrom.POST({
            url: URLC + '/Itinerary',
            data: model,
            success: function (result) {
                $('tbody#TblData').html('');
                $('#Pagination').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var currentPage = 1;
                        var pagination = result.paging;
                        if (pagination !== null) {
                            totalPage = pagination.TotalPage;
                            currentPage = pagination.Page;
                            pageSize = pagination.PageSize;
                            pageIndex = pagination.Page;
                        }
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            //
                            var pnr = item.PNR;
                            var issueDate = item.IssueDateText;
                            var airlineId = item.AirlineID;
                            var ticketingId = item.TicketingID;
                            var ticketingName = item.TicketingName;
                            var agentCode = item.AgentCode;
                            var itineraryText = item.ItineraryText;
                            //var customerTypeText = item.CustomerTypeText;
                            //var companyId = item.CompanyID;
                            //var companyCode = item.CompanyCode;
                            //var contactName = item.ContactName;
                            //
                            var originLocation = item.OriginLocation;
                            var destinationLocation = item.DestinationLocation;

                            var totalAmount = item.TotalAmount;
                            var _unit = 'đ';
                            //  role
                            var action = HelperModel.RolePermission(result.role, "AirBook", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr data-id='${id}' data-ticketingId='${ticketingId}'>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td >${issueDate}</td>  
                                 <td class='text-center'>${agentCode}</td>  
                                 <td >${ticketingName}</td>  
                                 <td ><a class='btn-passenger' data-id='${id}'>C.tiết</a></td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                 <td class='text-center bg-success'>${airlineId}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                                 <td class='text-left'>${itineraryText}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
                                 <td class='text-left bg-yellow-1'>${originLocation}-${destinationLocation}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
                                 <td class='text-left'>${pnr}</td>                                                                                                                                                                                                                                                                         
                                 <td class='tbcol-left tbcol-button text-center'>
                                     <button type="button" class="btn btn-danger btn-sm btn-voiditinerary" data-id="${id}" data-pnr="${pnr}">Hủy</button>
                                 </td>                                  
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AirBookItineraryController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    VoidItinerary: function (id) {
        var model = {
            ID: id
        };
        AjaxFrom.POST({
            url: URLC + '/VoidItinerary',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        AirBookItineraryController.DataList(pageIndex);
                        return;
                    }
                    else {
                        Notifization.Error(response.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    ConfirmVoidItinerary: function (id) {
        Confirm.ConfirmYN(id, AirBookItineraryController.VoidItinerary, Confirm.Text_VoidItinerary);
    }
};
//
AirBookItineraryController.init();


$(document).on("click", ".btn-passenger", function () {
    var id = $(this).data("id");
    var model = {
        ID: id
    };
    //
    $('#PassengerModal tbody#TblModalData').html('');
    AjaxFrom.POST({
        url: '/Management/AirOrder/Action/GetPassenger',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    var rowData = '';
                    var cnt = 1;
                    $.each(result.data, function (index, item) {
                        index = index + 1;
                        var id = item.ID;
                        if (id.length > 0)
                            id = id.trim();
                        //
                        var name = item.FullName;
                        var gender = item.GenderText;
                        var dateOfBirth = item.DateOfBirth;
                        var passengerType = item.PassengerType;
                        // 
                        rowData += `
                            <tr>
                                 <td class="text-right">${cnt}&nbsp;</td>  
                                 <td>${name}</td>  
                                 <td>${passengerType}</td>  
                                 <td>${gender}</td>  
                                 <td>${dateOfBirth}</td>   
                            </tr>`;
                        cnt++;
                    });
                    $('#PassengerModal tbody#TblModalData').html(rowData);
                    $("#PassengerModal").modal();
                    return;
                }
                else {
                    Notifization.Error(result.message);
                    return;
                }
            }
            Notifization.Error(MessageText.NotService);
            return;
        },
        error: function (result) {
            console.log('::' + MessageText.NotService);
        }
    });
});

$(document).on("click", ".btn-book-void", function () {
    var id = $(this).data("id");
    AirBookItineraryController.ConfirmVoidItinerary(id);

})