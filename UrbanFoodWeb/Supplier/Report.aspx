<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="UrbanFoodWeb.Supplier.Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <form id="form1" runat="server">
        <div class="row">
            <!-- Sales Summary Card -->
            <div class="col-lg-3 col-md-6">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                                    Total Sales (Monthly)</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblMonthlySales" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i data-feather="dollar-sign" class="feather-icon"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Orders Summary Card -->
            <div class="col-lg-3 col-md-6">
                <div class="card border-left-success shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                                    Total Orders (Monthly)</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblMonthlyOrders" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i data-feather="shopping-cart" class="feather-icon"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Product Count Card -->
            <div class="col-lg-3 col-md-6">
                <div class="card border-left-info shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-info text-uppercase mb-1">
                                    Active Products</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblActiveProducts" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i data-feather="box" class="feather-icon"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Average Rating Card -->
            <div class="col-lg-3 col-md-6">
                <div class="card border-left-warning shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
                                    Average Rating</div>
                                <div class="row no-gutters align-items-center">
                                    <div class="col-auto">
                                        <div class="h5 mb-0 mr-3 font-weight-bold text-gray-800">
                                            <asp:Label ID="lblAverageRating" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col">
                                        <div class="progress progress-sm mr-2">
                                            <div id="ratingProgressBar" runat="server" class="progress-bar bg-warning" role="progressbar"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i data-feather="star" class="feather-icon"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Date Range Filter -->
        <div class="row mt-4">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Filter Reports</h4>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="txtFromDate">From Date:</label>
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="txtToDate">To Date:</label>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <div>
                                        <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter" CssClass="btn btn-primary" OnClick="btnApplyFilter_Click" />
                                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

      

        <!-- Top Products and Categories -->
        <div class="row mt-4">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Top Selling Products</h4>
                        <div class="table-responsive">
                            <asp:GridView ID="gvTopProducts" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered">
                                <Columns>
                                    <asp:BoundField DataField="ProductID" HeaderText="ID" />
                                    <asp:BoundField DataField="ProductName" HeaderText="Product" />
                                    <asp:BoundField DataField="TotalQuantity" HeaderText="Quantity Sold" />
                                    <asp:BoundField DataField="TotalSales" HeaderText="Total Sales" DataFormatString="{0:C}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Sales by Category</h4>
                        <div class="table-responsive">
                            <asp:GridView ID="gvCategorySales" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered">
                                <Columns>
                                    <asp:BoundField DataField="CategoryID" HeaderText="ID" />
                                    <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                    <asp:BoundField DataField="ProductCount" HeaderText="Products" />
                                    <asp:BoundField DataField="TotalSales" HeaderText="Total Sales" DataFormatString="{0:C}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Recent Orders -->
        <div class="row mt-4">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Recent Orders</h4>
                        <div class="table-responsive">
                            <asp:GridView ID="gvRecentOrders" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered">
                                <Columns>
                                    <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                    <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="TotalAmount" HeaderText="Amount" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Download Reports Section -->
        <div class="row mt-4">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Download Reports</h4>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="ddlReportType">Report Type:</label>
                                    <asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-control">
                                      
                                        <asp:ListItem Text="Product Sales Report" Value="ProductSalesReport" />
                                        <asp:ListItem Text="Category Sales Report" Value="CategorySalesReport" />
                                        <asp:ListItem Text="Order Status Report" Value="OrderStatusReport" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="ddlReportFormat">Report Format:</label>
                                    <asp:DropDownList ID="ddlReportFormat" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Excel" Value="Excel" />
                                        <asp:ListItem Text="PDF" Value="PDF" />
                                        <asp:ListItem Text="CSV" Value="CSV" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <div>
                                        <asp:Button ID="btnDownloadReport" runat="server" Text="Download Report" CssClass="btn btn-success" OnClick="btnDownloadReport_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Chart scripts -->
        <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
        </form>
</asp:Content>