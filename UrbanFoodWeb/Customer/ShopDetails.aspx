<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="ShopDetails.aspx.cs" Inherits="UrbanFoodWeb.Customer.ShopDetails" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .star-rating .fa {
            cursor: pointer;
            color: #ddd;
        }
        .star-rating .fa.checked {
            color: #ffcc00;
        }
        .review-container {
            border-bottom: 1px solid #eee;
            padding: 15px 0;
        }
        .review-header {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
        }
        .review-stars .fa-star.checked {
            color: #ffcc00;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <!-- Single Page Header start -->
        <div class="container-fluid page-header py-5">
            <h1 class="text-center text-white display-6">Shop Detail</h1>
            <ol class="breadcrumb justify-content-center mb-0">
                <li class="breadcrumb-item"><a href="#">Home</a></li>
                
                <li class="breadcrumb-item active text-white">Shop Detail</li>
            </ol>
        </div>
        <!-- Single Page Header End -->


        <!-- Single Product Start -->
        <div class="container-fluid py-5 mt-5">
            <div class="container py-5">
                <div class="row g-4 mb-5">
                    <div class="col-lg-8 col-xl-9">
                        <div class="row g-4">
                            <div class="col-lg-6">
                                <div class="border rounded">
                                    <asp:Image ID="imgProduct" runat="server" CssClass="img-fluid rounded" ImageUrl="../CustomerTemplate/img/single-item.jpg" />
                                </div>
                            </div>
                           <div class="col-lg-6">
                                <h4 class="fw-bold mb-3">
                                    <asp:Label ID="lblProductName" runat="server" Text="Product Name"></asp:Label>
                                </h4>
                                <p class="mb-3">
                                    Category: <asp:Label ID="lblCategory" runat="server" Text="Category"></asp:Label>
                                </p>
                               
                                <h5 class="fw-bold mb-3">
                                    Price: <asp:Label ID="lblPrice" runat="server" Text="$0.00"></asp:Label>
                                </h5>
                                
                               
                              

                                <asp:HiddenField ID="hdnProductId" runat="server" />
                            </div>

                            <div class="col-lg-12">
                                <nav>
                                    <div class="nav nav-tabs mb-3">
                                        <button class="nav-link active border-white border-bottom-0" type="button" role="tab"
                                            id="nav-about-tab" data-bs-toggle="tab" data-bs-target="#nav-about"
                                            aria-controls="nav-about" aria-selected="true">Description</button>
                                        <button class="nav-link border-white border-bottom-0" type="button" role="tab"
                                            id="nav-mission-tab" data-bs-toggle="tab" data-bs-target="#nav-mission"
                                            aria-controls="nav-mission" aria-selected="false">Reviews</button>
                                    </div>
                                </nav>
                                <div class="tab-content mb-5">
                                    <div class="tab-pane active" id="nav-about" role="tabpanel" aria-labelledby="nav-about-tab">
                                        <p class="mb-4">
                                            <asp:Label ID="lblDescription" runat="server" />
                                        </p>
                                    </div>
                                    
                                    <div class="tab-pane" id="nav-mission" role="tabpanel" aria-labelledby="nav-mission-tab">
                                        <!-- Reviews Section -->
                                        <div class="row">
                                            <div class="col-12">
                                                <h5 class="mb-4">Product Reviews</h5>
                                                <div class="d-flex align-items-center mb-4">
                                                    <h4 class="me-2"><asp:Label ID="lblAverageRating" runat="server" Text="0.0"></asp:Label></h4>
                                                    <div class="review-stars">
                                                        <i class="fa fa-star"></i>
                                                        <i class="fa fa-star"></i>
                                                        <i class="fa fa-star"></i>
                                                        <i class="fa fa-star"></i>
                                                        <i class="fa fa-star"></i>
                                                    </div>
                                                    <span class="ms-2">(<asp:Label ID="lblReviewCount" runat="server" Text="0"></asp:Label> reviews)</span>
                                                </div>
                                                
                                                <!-- Review list placeholder -->
                                                <asp:Repeater ID="rptReviews" runat="server">
                                                    <ItemTemplate>
                                                        <div class="review-container">
                                                            <div class="review-header">
                                                                <div>
                                                                    <h6><%# Eval("CustomerName") %></h6>
                                                                    <small class="text-muted"><%# ((DateTime)Eval("CreatedDate")).ToString("MMM dd, yyyy") %></small>
                                                                </div>
                                                                <div class="review-stars">
                                                                    <asp:Literal ID="litStars" runat="server" Text='<%# GetStarRating((int)Eval("Rating")) %>'></asp:Literal>
                                                                </div>
                                                            </div>
                                                            <p><%# Eval("ReviewText") %></p>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Review Form -->
                            <div class="col-12">
                                <h4 class="mb-4 fw-bold">Leave a Review</h4>
                                <div class="row g-4">
                                    <div class="col-lg-6">
                                        <div class="border-bottom rounded">
                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control border-0 me-4" placeholder="Your Name *"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="border-bottom rounded">
                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control border-0" placeholder="Your Email *" TextMode="Email"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="border-bottom rounded my-4">
                                            <asp:TextBox ID="txtReview" runat="server" CssClass="form-control border-0" TextMode="MultiLine" Rows="8" placeholder="Your Review *"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="d-flex justify-content-between py-3 mb-5">
                                            <div class="d-flex align-items-center">
                                                <p class="mb-0 me-3">Please rate:</p>
                                                <div class="star-rating d-flex align-items-center">
                                                    <i class="fa fa-star" data-rating="1"></i>
                                                    <i class="fa fa-star" data-rating="2"></i>
                                                    <i class="fa fa-star" data-rating="3"></i>
                                                    <i class="fa fa-star" data-rating="4"></i>
                                                    <i class="fa fa-star" data-rating="5"></i>
                                                </div>
                                                <asp:HiddenField ID="hdnRating" runat="server" Value="0" />
                                            </div>
                                            <asp:Button ID="btnSubmitReview" runat="server" CssClass="btn border border-secondary text-primary rounded-pill px-4 py-3" Text="Post Review" OnClick="btnSubmitReview_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Single Product End -->

    <script type="text/javascript">
        $(document).ready(function() {
            // Quantity buttons functionality
            $('.btn-minus').on('click', function(e) {
                e.preventDefault();
                var input = $(this).closest('.quantity').find('input');
                var value = parseInt(input.val());
                if (value > 1) {
                    input.val(value - 1);
                }
                return false;
            });

            $('.btn-plus').on('click', function(e) {
                e.preventDefault();
                var input = $(this).closest('.quantity').find('input');
                var value = parseInt(input.val());
                input.val(value + 1);
                return false;
            });

            // Star rating functionality
            $('.star-rating .fa').on('click', function() {
                var rating = $(this).data('rating');
                $('#<%= hdnRating.ClientID %>').val(rating);
                
                // Update the stars
                $('.star-rating .fa').removeClass('checked');
                $('.star-rating .fa').each(function() {
                    if ($(this).data('rating') <= rating) {
                        $(this).addClass('checked');
                    }
                });
            });

            // Highlight stars on hover
            $('.star-rating .fa').hover(
                function() {
                    var currentRating = $(this).data('rating');
                    $('.star-rating .fa').each(function() {
                        if ($(this).data('rating') <= currentRating) {
                            $(this).addClass('checked');
                        }
                    });
                },
                function() {
                    var selectedRating = $('#<%= hdnRating.ClientID %>').val();
                    $('.star-rating .fa').removeClass('checked');
                    $('.star-rating .fa').each(function() {
                        if ($(this).data('rating') <= selectedRating) {
                            $(this).addClass('checked');
                        }
                    });
                }
            );

            // Setup tabs
            $('#nav-mission-tab').on('click', function() {
                $('#nav-mission').addClass('active');
                $('#nav-about').removeClass('active');
                $(this).addClass('active');
                $('#nav-about-tab').removeClass('active');
            });

            $('#nav-about-tab').on('click', function() {
                $('#nav-about').addClass('active');
                $('#nav-mission').removeClass('active');
                $(this).addClass('active');
                $('#nav-mission-tab').removeClass('active');
            });
        });
    </script>

</asp:Content>