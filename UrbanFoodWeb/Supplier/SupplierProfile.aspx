<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.master" AutoEventWireup="true" CodeBehind="SupplierProfile.aspx.cs" Inherits="UrbanFoodWeb.Supplier.SupplierProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Supplier Profile</h4>
                        <p class="card-subtitle mb-4">Update your profile information below</p>
                        
                        <div class="alert alert-success" id="successAlert" runat="server" visible="false">
                            <strong>Success!</strong> Your profile was updated successfully.
                        </div>
                        <div class="alert alert-danger" id="errorAlert" runat="server" visible="false">
                            <strong>Error!</strong> <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtFirstName">First Name</label>
                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="First Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                                        ControlToValidate="txtFirstName" 
                                        ErrorMessage="First Name is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtLastName">Last Name</label>
                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Last Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                                        ControlToValidate="txtLastName" 
                                        ErrorMessage="Last Name is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtEmail">Email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email" TextMode="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                        ControlToValidate="txtEmail" 
                                        ErrorMessage="Email is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                        ControlToValidate="txtEmail"
                                        ErrorMessage="Please enter a valid email address"
                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                        CssClass="text-danger"
                                        Display="Dynamic">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtPhoneNumber">Phone Number</label>
                                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" placeholder="Phone Number"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" 
                                        ControlToValidate="txtPhoneNumber" 
                                        ErrorMessage="Phone Number is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="txtAddress">Address</label>
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Address" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAddress" runat="server" 
                                        ControlToValidate="txtAddress" 
                                        ErrorMessage="Address is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtBusinessName">Business Name</label>
                                    <asp:TextBox ID="txtBusinessName" runat="server" CssClass="form-control" placeholder="Business Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvBusinessName" runat="server" 
                                        ControlToValidate="txtBusinessName" 
                                        ErrorMessage="Business Name is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="ddlBusinessType">Business Type</label>
                                    <asp:DropDownList ID="ddlBusinessType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="-- Select Business Type --" Value="" />
                                        <asp:ListItem Text="Farmer" Value="Restaurant" />
                                        <asp:ListItem Text="Grocery Store" Value="Grocery Store" />
                                        <asp:ListItem Text="Baked Goods" Value="Bakery" />
                                        <asp:ListItem Text="Handmade Crafts" Value="Fast Food" />
                                        <asp:ListItem Text="Dairy products" Value="Catering" />
                                        <asp:ListItem Text="Juices" Value="Food Truck" />
                                        <asp:ListItem Text="Other" Value="Other" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvBusinessType" runat="server" 
                                        ControlToValidate="ddlBusinessType" 
                                        ErrorMessage="Business Type is required" 
                                        CssClass="text-danger" 
                                        Display="Dynamic"
                                        InitialValue="">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="col-12">
                                <h5>Change Password (Leave blank to keep current password)</h5>
                                <hr />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtCurrentPassword">Current Password</label>
                                    <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" placeholder="Current Password" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtNewPassword">New Password</label>
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" placeholder="New Password" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtConfirmPassword">Confirm New Password</label>
                                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="Confirm New Password" TextMode="Password"></asp:TextBox>
                                    <asp:CompareValidator ID="cvPassword" runat="server" 
                                        ControlToValidate="txtConfirmPassword"
                                        ControlToCompare="txtNewPassword"
                                        ErrorMessage="Passwords do not match"
                                        CssClass="text-danger"
                                        Display="Dynamic">
                                    </asp:CompareValidator>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="col-md-12">
                                <asp:Button ID="btnUpdateProfile" runat="server" Text="Update Profile" CssClass="btn btn-primary" OnClick="btnUpdateProfile_Click" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary ml-2" CausesValidation="false" OnClick="btnCancel_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>