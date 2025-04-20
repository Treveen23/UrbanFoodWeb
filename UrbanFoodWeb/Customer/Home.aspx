<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="UrbanFoodWeb.Customer.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      
         <!-- Modal Search Start -->
    <div class="modal fade" id="searchModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content rounded-0">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Search by keyword</h5>
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


    <!-- Hero Start -->
   

    <div class="container-fluid py-5 mb-5 hero-header">
        <div class="container py-5">
            <div class="row g-5 align-items-center">
                <asp:Label ID="lblWelcome" runat="server" CssClass="h4 text-success"></asp:Label>
                <div class="col-md-12 col-lg-7">
                    <h4 class="mb-3 text-secondary">100% Fresh from Nature</h4>
                    <h2 class="mb-5 display-3 text-primary">Organic Veggies,Fruits,Baked Goods & Handmade Crafts</h2>
                  


                   </div>
                <div class="col-md-12 col-lg-5">
                    <div id="carouselId" class="carousel slide position-relative" data-bs-ride="carousel">
                        <div class="carousel-inner" role="listbox">
                            <div class="carousel-item active rounded">
                                <img src="../CustomerTemplate/img/hero-img-1.png" class="img-fluid w-100 h-100 bg-secondary rounded" alt="First slide">
                                <a href="#" class="btn px-4 py-2 text-white rounded">Fruits</a>
                            </div>
                            <div class="carousel-item rounded">
                                <img src="../CustomerTemplate/img/hero-img-2.jpg" class="img-fluid w-100 h-100 rounded" alt="Second slide">
                                <a href="#" class="btn px-4 py-2 text-white rounded">Vegitables</a>
                            </div>
                            <div class="carousel-item rounded">
    <img src="../CustomerTemplate/img/hero-img3.jpeg" class="img-fluid w-100 h-100 rounded" alt="Second slide">
    <a href="#" class="btn px-4 py-2 text-white rounded">Dairy Products</a>
</div>
                            <div class="carousel-item rounded">
    <img src="../CustomerTemplate/img/hero-img4.jpg" class="img-fluid w-100 h-100 rounded" alt="Second slide">
    <a href="#" class="btn px-4 py-2 text-white rounded">Baked Goods</a>
</div>
                            <div class="carousel-item rounded">
    <img src="../CustomerTemplate/img/hero-img5.jpeg" class="img-fluid w-100 h-100 rounded" alt="Second slide">
    <a href="#" class="btn px-4 py-2 text-white rounded">Handmade Crafts</a>
</div>
                        </div>
                        <button class="carousel-control-prev" type="button" data-bs-target="#carouselId" data-bs-slide="prev">
                            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                            <span class="visually-hidden">Previous</span>
                        </button>
                        <button class="carousel-control-next" type="button" data-bs-target="#carouselId" data-bs-slide="next">
                            <span class="carousel-control-next-icon" aria-hidden="true"></span>
                            <span class="visually-hidden">Next</span>
                        </button>
                       
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Hero End -->
    <!-- Featurs Section Start -->
<div class="container-fluid featurs py-5">
    <div class="container py-5">
        <div class="row g-4">
            <div class="col-md-6 col-lg-3">
                <div class="featurs-item text-center rounded bg-light p-4">
                    <div class="featurs-icon btn-square rounded-circle bg-secondary mb-5 mx-auto">
                        <i class="fas fa-car-side fa-3x text-white"></i>
                    </div>
                    <div class="featurs-content text-center">
                        <h5>Free Shipping</h5>
                        <p class="mb-0"> On order over Rs.2000</p>
                    </div>
                </div>
            </div>
            <div class="col-md-6 col-lg-3">
                <div class="featurs-item text-center rounded bg-light p-4">
                    <div class="featurs-icon btn-square rounded-circle bg-secondary mb-5 mx-auto">
                        <i class="fas fa-user-shield fa-3x text-white"></i>
                    </div>
                    <div class="featurs-content text-center">
                        <h5>Security Payment</h5>
                        <p class="mb-0">100% safe & secure Checkout</p>
                    </div>
                </div>
            </div>
            <div class="col-md-6 col-lg-3">
                <div class="featurs-item text-center rounded bg-light p-4">
                    <div class="featurs-icon btn-square rounded-circle bg-secondary mb-5 mx-auto">
                        <i class="fas fa-exchange-alt fa-3x text-white"></i>
                    </div>
                    <div class="featurs-content text-center">
                        <h5>Money-back Guarantee</h5>
                        <p class="mb-0">30 day money guarantee</p>
                    </div>
                </div>
            </div>
            <div class="col-md-6 col-lg-3">
                <div class="featurs-item text-center rounded bg-light p-4">
                    <div class="featurs-icon btn-square rounded-circle bg-secondary mb-5 mx-auto">
                        <i class="fa fa-phone-alt fa-3x text-white"></i>
                    </div>
                    <div class="featurs-content text-center">
                        <h5>24/7 Support</h5>
                        <p class="mb-0">Fast & Friendly Customer Service</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Featurs Section End -->

  <!-- Category Filter Start -->
<div class="container-fluid py-5 bg-light">
    <div class="container">
        <div class="text-center mx-auto mb-5" style="max-width: 700px;">
            <h1 class="display-4">Shop By Category</h1>
            <p class="text-primary">Browse our fresh products by category</p>
        </div>
        <div class="row g-4 justify-content-center">
            <div class="col-12 text-center">
                <asp:LinkButton ID="btnAllCategories" runat="server" CssClass="btn btn-secondary rounded-pill px-4 me-2 mb-3 active" 
                    CommandName="FilterCategory" CommandArgument="0" OnCommand="FilterCategory_Command">
                    All Products
                </asp:LinkButton>
                <asp:Repeater ID="rptCategories" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnCategory" runat="server" CssClass="btn btn-outline-secondary rounded-pill px-4 me-2 mb-3"
                            CommandName="FilterCategory" CommandArgument='<%# Eval("CategoryId") %>' OnCommand="FilterCategory_Command">
                            <%# Eval("CategoryName") %>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>
<!-- Category Filter End -->


    

   <!-- Fruits Shop Start -->
<div class="container-fluid fruite py-5">
    <div class="container py-5">
        <div class="tab-class text-center">
            <div class="row g-4 justify-content-center">
                <div class="col-lg-10 text-center">
                    <h1>Our Organic Products</h1>
                    <!-- Products Grid Start -->
                    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 g-4 justify-content-center">
                        <asp:Repeater ID="rptProducts" runat="server">
                            <ItemTemplate>
                                <div class="col">
                                    <div class="card h-100">
                                        <asp:Image ID="imgProduct" runat="server" ImageUrl='<%# Eval("ImageURL") %>' CssClass="card-img-top" AlternateText='<%# Eval("ProductName") %>' />
                                        <div class="card-body d-flex flex-column">
                                            <h5 class="card-title"><%# Eval("ProductName") %></h5>
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
                    <!-- Products Grid End -->
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Fruits Shop End -->



  

    


   <!-- Banner Section Start-->
<div class="container-fluid banner bg-secondary my-5">
    <div class="container py-5">
        <div class="row g-4 align-items-center">
            <div class="col-lg-6">
                <div class="py-4">
                    <h1 class="display-3 text-white">Fresh Exotic Fruits</h1>
                    <p class="fw-normal display-3 text-dark mb-4">in Our Store</p>
                    <p class="mb-4 text-dark">Naturally grown,freshly picked exotic fruits full of flavor and goodness</p>
                    <asp:Button ID="btnBuy" runat="server" CssClass="banner-btn btn border-2 border-white rounded-pill text-dark py-3 px-5" Text="BUY" OnClick="btnBuy_Click" />
                </div>
            </div>
            <div class="col-lg-6">
                <div class="position-relative">
                    <img src="../CustomerTemplate/img/baner-1.png" class="img-fluid w-100 rounded" alt="">
                    <div class="d-flex align-items-center justify-content-center bg-white rounded-circle position-absolute" style="width: 140px; height: 140px; top: 0; left: 0;">
                        <h1 style="font-size: 100px;">1</h1>
                        <div class="d-flex flex-column">
                            <span class="h2 mb-0">50$</span>
                            <span class="h4 text-muted mb-0">kg</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Banner Section End -->

    <!-- Farm Stories Start -->
<div class="container-fluid py-5">
    <div class="container">
        <div class="text-center mx-auto mb-5" style="max-width: 700px;">
            <h1 class="display-4">From Our Farms To Your Table</h1>
            <p class="text-primary">Meet the farmers behind your favorite products</p>
        </div>
        <div class="row g-4">
            <div class="col-lg-4 col-md-6">
                <div class="farm-story-item position-relative bg-light rounded p-4">
                    <div class="d-flex align-items-center mb-4">
                        <div class="farm-story-img rounded-circle overflow-hidden">
                            <img src="../CustomerTemplate/img/farmer1.jpeg" class="img-fluid" alt="Farmer">
                        </div>
                        <div class="ms-4">
                            <h5 class="text-primary">John's Family Farm</h5>
                            <span class="text-muted">Organic Vegetables</span>
                        </div>
                    </div>
                    <p class="mb-3">Our vegetables are grown with sustainable practices that have been in our family for generations. We never use pesticides and harvest everything at peak ripeness.</p>
                    
                </div>
            </div>
            <div class="col-lg-4 col-md-6">
                <div class="farm-story-item position-relative bg-light rounded p-4">
                    <div class="d-flex align-items-center mb-4">
                        <div class="farm-story-img rounded-circle overflow-hidden">
                            <img src="../CustomerTemplate/img/farmer2.jpg" class="img-fluid" alt="Farmer">
                        </div>
                        <div class="ms-4">
                            <h5 class="text-primary">Green Valley Dairy</h5>
                            <span class="text-muted">Organic Dairy Products</span>
                        </div>
                    </div>
                    <p class="mb-3">Our dairy cows roam freely on lush green pastures, producing milk that's rich in nutrients and flavor. We craft our artisanal cheeses and yogurts with traditional methods.</p>
                    
                </div>
            </div>
            <div class="col-lg-4 col-md-6">
    <div class="farm-story-item position-relative bg-light rounded p-4">
        <div class="d-flex align-items-center mb-4">
            <div class="farm-story-img rounded-circle overflow-hidden">
                <img src="../CustomerTemplate/img/farmer3.jpeg" class="img-fluid" alt="Baker">
            </div>
            <div class="ms-4">
                <h5 class="text-primary">Countryside Bakery</h5>
                <span class="text-muted">Artisanal Baked Goods</span>
            </div>
        </div>
        <p class="mb-3">Our bakery uses traditional methods passed down through generations. We bake everything fresh daily using locally-sourced organic ingredients to create breads, pastries, and treats that are both delicious and wholesome.</p>
    </div>
</div>
        </div>
    </div>
</div>
<!-- Farm Stories End -->


</asp:Content>
