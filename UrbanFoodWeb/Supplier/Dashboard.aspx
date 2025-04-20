<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="UrbanFoodWeb.Supplier.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dashboard-card {
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            margin-bottom: 20px;
            transition: transform 0.3s;
        }
        .dashboard-card:hover {
            transform: translateY(-5px);
        }
        .card-icon {
            font-size: 2.5rem;
            color: #fff;
            background: linear-gradient(45deg, #4CAF50, #8BC34A);
            border-radius: 50%;
            width: 60px;
            height: 60px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 15px;
        }
        .stat-title {
            color: #666;
            font-size: 14px;
            margin-bottom: 5px;
        }
        .stat-value {
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 0;
        }
        .chart-container {
            height: 300px;
            margin-bottom: 20px;
        }
        .recent-orders-table th {
            background-color: #f8f9fa;
        }
        .badge-status-pending {
            background-color: #ffc107;
        }
        .badge-status-delivered {
            background-color: #28a745;
        }
        .badge-status-cancelled {
            background-color: #dc3545;
        }
        .dashboard-link {
            text-decoration: none;
            color: inherit;
        }
        .dashboard-link:hover {
            color: #4CAF50;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- Welcome Banner -->
    <form id="form1" runat="server">
    <div class="row mb-4">
        <div class="col-12">
            <div class="card dashboard-card">
                <div class="card-body">
                    <div class="d-flex align-items-center">
                        <div>
                            <h4 class="card-title mb-0">Welcome back, <asp:Literal ID="ltSupplierName" runat="server"></asp:Literal>!</h4>
                            <p class="text-muted">Here's your business overview for today, <asp:Literal ID="ltCurrentDate" runat="server"></asp:Literal></p>
                        </div>
                        <div class="ml-auto">
                            <asp:LinkButton ID="btnRefresh" runat="server" CssClass="btn btn-outline-success" OnClick="btnRefresh_Click">
                                <i class="fas fa-sync-alt"></i> Refresh Data
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

   
      
    <!-- Charts Section -->
    <div class="row">
        <!-- Sales Chart -->
        <div class="col-md-8">
            <div class="card dashboard-card">
                <div class="card-body">
                    <h5 class="card-title">Sales Performance</h5>
                    <div class="chart-container">
                        <canvas id="salesChart"></canvas>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Product Distribution Chart -->
        <div class="col-md-4">
            <div class="card dashboard-card">
                <div class="card-body">
                    <h5 class="card-title">Products by Category</h5>
                    <div class="chart-container">
                        <canvas id="categoryChart"></canvas>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Orders Table -->
    <div class="row">
        <div class="col-12">
            <div class="card dashboard-card">
                <div class="card-body">
                    <div class="d-flex align-items-center mb-4">
                        <h5 class="card-title mb-0">Recent Orders</h5>
                        <div class="ml-auto">
                            <a href="Order.aspx" class="btn btn-sm btn-outline-success">View All Orders</a>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvRecentOrders" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-striped table-hover recent-orders-table" 
                            EmptyDataText="No recent orders found">
                            <Columns>
                                <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
                                <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField DataField="TotalAmount" HeaderText="Amount" DataFormatString="${0:F2}" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='badge <%# GetStatusBadgeClass(Eval("OrderStatus").ToString()) %>'>
                                            <%# Eval("OrderStatus") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Low Stock Alerts -->
    <div class="row">
        <div class="col-12">
            <div class="card dashboard-card">
                <div class="card-body">
                    <div class="d-flex align-items-center mb-4">
                        <h5 class="card-title mb-0">
                            <i class="fas fa-exclamation-triangle text-warning mr-2"></i>
                            Low Stock Alerts
                        </h5>
                        <div class="ml-auto">
                            <a href="ProductManagement.aspx" class="btn btn-sm btn-outline-warning">Manage Inventory</a>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvLowStock" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-striped table-hover" 
                            EmptyDataText="No low stock items found">
                            <Columns>
                                <asp:BoundField DataField="ProductID" HeaderText="Product ID" />
                                <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                <asp:BoundField DataField="QuantityAvailable" HeaderText="Current Stock" />
                               
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- JavaScript for Charts -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.7.0/chart.min.js"></script>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Sales Chart
            var salesCtx = document.getElementById('salesChart').getContext('2d');
            var salesData = <%= GetSalesChartData() %>;
            
            var salesChart = new Chart(salesCtx, {
                type: 'line',
                data: {
                    labels: salesData.labels,
                    datasets: [{
                        label: 'Sales',
                        data: salesData.values,
                        backgroundColor: 'rgba(76, 175, 80, 0.2)',
                        borderColor: '#4CAF50',
                        borderWidth: 2,
                        tension: 0.3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                callback: function(value) {
                                    return '$' + value;
                                }
                            }
                        }
                    }
                }
            });
            
            // Category Chart
            var categoryCtx = document.getElementById('categoryChart').getContext('2d');
            var categoryData = <%= GetCategoryChartData() %>;
            
            var categoryChart = new Chart(categoryCtx, {
                type: 'doughnut',
                data: {
                    labels: categoryData.labels,
                    datasets: [{
                        data: categoryData.values,
                        backgroundColor: [
                            '#4CAF50', '#2196F3', '#FF9800', '#9C27B0', 
                            '#00BCD4', '#F44336', '#FFEB3B', '#795548'
                        ]
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'right'
                        }
                    }
                }
            });
        });
    </script>
        </form>
</asp:Content>