<%@ Page Title="Shop" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs" Inherits="UrbanFoodWeb.Customer.Shop" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Include any additional CSS or JS here -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

       <!-- Modal Search Start -->
    <div class="modal fade" id="searchModal" tabindex="-1" aria-labelledby="searchModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content rounded-0">
                <div class="modal-header">
                    <h5 class="modal-title" id="searchModalLabel">Search by keyword</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body d-flex align-items-center">
                    <div class="input-group w-75 mx-auto d-flex">
                        <input type="search" class="form-control p-3" placeholder="keywords" aria-describedby="search-icon-1">
                        <span id="search-icon-1" class="input-group-text p-3"><i class="fa fa-search"></i></span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal Search End -->

    <!-- Page Header Start -->
    <div class="container-fluid page-header py-5">
        <h1 class="text-center text-white display-6">Shop</h1>
        <ol class="breadcrumb justify-content-center mb-0">
            <li class="breadcrumb-item"><a href="#">Home</a></li>
            <li class="breadcrumb-item active text-white">Shop</li>
        </ol>
    </div>
    <!-- Page Header End -->

    <!-- Shop Section Start -->
    <div class="container-fluid fruite py-5">
        <div class="container py-5">
            <div class="row g-4">
                <!-- Categories Sidebar Start -->
                <div class="col-lg-3">
                    <div class="mb-4">
                        <h4>Categories</h4>
                        <ul class="list-unstyled fruite-categorie">
                            <asp:Repeater ID="rptCategories" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <div class="d-flex justify-content-between fruite-name">
                                            <a href="Shop.aspx?categoryId=<%# Eval("CategoryID") %>">
                                                <i class="fas fa-apple-alt me-2"></i><%# Eval("CategoryName") %>
                                            </a>
                                            <span>(<%# Eval("ProductCount") %>)</span>
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </div>
                <!-- Categories Sidebar End -->

                <!-- Products Grid Start -->
                <div class="col-lg-9">
                    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 g-4">
                     <asp:Repeater ID="rptProducts" runat="server">
    <ItemTemplate>
        <div class="col">
            <div class="card h-100">
                <!-- Make image clickable -->
                <a href='ShopDetails.aspx?ProductID=<%# Eval("ProductID") %>'>
                    <asp:Image ID="imgProduct" runat="server" ImageUrl='<%# Eval("ImageURL") %>' CssClass="card-img-top" AlternateText='<%# Eval("ProductName") %>' />
                </a>
                <div class="card-body d-flex flex-column">
                    <!-- Make title clickable -->
                    <h5 class="card-title">
                        <a href='~/Customer/ShopDetails.aspx?ProductID=<%# Eval("ProductID") %>' class="text-decoration-none text-dark">
                            <%# Eval("ProductName") %>
                        </a>
                    </h5>
                    <p class="card-text"><%# Eval("Description") %></p>
                    <div class="mt-auto d-flex justify-content-between align-items-center">
                        <span class="fw-bold text-dark">$<%# Eval("Price") %> / kg</span>
                        <asp:LinkButton ID="btnAddToCart" runat="server" CssClass="btn btn-outline-primary btn-sm"
                            CommandName="AddToCart" CommandArgument='<%# Eval("ProductID") %>' OnCommand="btnAddToCart_Command">
                            <i class="fa fa-shopping-bag me-2"></i> Add to cart
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
                    </div>
                </div>
                <!-- Products Grid End -->
            </div>
        </div>
    </div>
    <!-- Shop Section End -->


</asp:Content>
