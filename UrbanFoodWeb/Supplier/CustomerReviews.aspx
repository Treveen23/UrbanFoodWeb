<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="CustomerReviews.aspx.cs" Inherits="UrbanFoodWeb.Supplier.CustomerReviews" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <style type="text/css">
        .rating {
            color: #FFD700;
            font-size: 18px;
        }
        .review-card {
            margin-bottom: 15px;
            border-left: 3px solid #4e73df;
        }
    </style>
     
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Customer Reviews</h4>
                        <h6 class="card-subtitle">Monitor and manage product reviews</h6>
                        
                        <!-- Product Filter -->
                        <div class="form-group row mt-3">
                            <label class="col-md-2">Filter by Product:</label>
                            <div class="col-md-4">
                                <asp:DropDownList ID="ddlProductFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProductFilter_SelectedIndexChanged">
                                    <asp:ListItem Value="0">All Products</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <label class="col-md-2">Rating:</label>
                            <div class="col-md-2">
                                <asp:DropDownList ID="ddlRatingFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRatingFilter_SelectedIndexChanged">
                                    <asp:ListItem Value="0">All Ratings</asp:ListItem>
                                    <asp:ListItem Value="5">5 Stars</asp:ListItem>
                                    <asp:ListItem Value="4">4 Stars</asp:ListItem>
                                    <asp:ListItem Value="3">3 Stars</asp:ListItem>
                                    <asp:ListItem Value="2">2 Stars</asp:ListItem>
                                    <asp:ListItem Value="1">1 Star</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn btn-info" OnClick="btnRefresh_Click"/>
                            </div>
                        </div>
                        
                        <!-- Summary Stats -->
                        <div class="row mt-4">
                            <div class="col-md-3">
                                <div class="card bg-info text-white">
                                    <div class="card-body">
                                        <h5 class="card-title">Average Rating</h5>
                                        <h2><asp:Literal ID="litAvgRating" runat="server">0.0</asp:Literal> <small>/ 5</small></h2>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-success text-white">
                                    <div class="card-body">
                                        <h5 class="card-title">Total Reviews</h5>
                                        <h2><asp:Literal ID="litTotalReviews" runat="server">0</asp:Literal></h2>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-warning text-white">
                                    <div class="card-body">
                                        <h5 class="card-title">Recent Reviews</h5>
                                        <h2><asp:Literal ID="litRecentReviews" runat="server">0</asp:Literal> <small>this week</small></h2>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-danger text-white">
                                    <div class="card-body">
                                        <h5 class="card-title">Negative Reviews</h5>
                                        <h2><asp:Literal ID="litNegativeReviews" runat="server">0</asp:Literal> <small>(≤ 2 stars)</small></h2>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                       <!-- Review Listing -->
<div class="mt-4">
    <asp:Repeater ID="rptReviews" runat="server">
        <ItemTemplate>
            <div class="card review-card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <h5><%# Eval("ProductName") %></h5>
                            <div class="rating">
                                <%# GetStarRating(Convert.ToInt32(Eval("Rating"))) %>
                            </div>
                            <p class="mt-2"><%# Eval("ReviewText") %></p>
                            <small class="text-muted">By <%# Eval("CustomerName") %> on <%# Eval("ReviewDate", "{0:MMM dd, yyyy}") %></small>
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    
    <asp:Panel ID="pnlNoReviews" runat="server" CssClass="alert alert-info mt-3" Visible="false">
        <i data-feather="info" class="feather-icon"></i> No reviews found for the selected criteria.
    </asp:Panel>
    
    <!-- Pagination -->
    <div class="row mt-3">
        <div class="col-md-12">
            <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
                <ItemTemplate>
                    <asp:LinkButton ID="btnPage" runat="server" 
                        CommandName="Page" CommandArgument="<%# Container.DataItem %>"
                        CssClass='<%# Convert.ToInt32(Container.DataItem) == Convert.ToInt32(ViewState["CurrentPage"]) ? "btn btn-primary" : "btn btn-outline-primary" %>'>
                        <%# Container.DataItem %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
                        </div>
                    </div>
                </div>
            </div>
        </form>

</asp:Content>
