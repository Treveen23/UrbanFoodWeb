<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="UrbanFoodWeb.Customer.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <div class="container registration-container">
        <div class="row mb-4">
            <div class="col-12 text-center">
                <h2 class="mb-4">Create Your Account</h2>
                <p class="text-muted mb-4">Join our community of food lovers and suppliers</p>
                <asp:HiddenField ID="hfUserType" runat="server" />
                <div class="d-flex justify-content-center mb-4">
                    <asp:Button ID="btnCustomer" runat="server" Text="Customer" CssClass="btn btn-toggle toggle-active me-2" OnClick="btnCustomer_Click" CausesValidation="false"/>
                    <asp:Button ID="btnSupplier" runat="server" Text="Supplier" CssClass="btn btn-toggle toggle-inactive" OnClick="btnSupplier_Click" CausesValidation="false" />

                </div>
            </div>
        </div>

        <!-- Common Registration Fields -->
        <div class="row mb-3">
            <div class="col-md-6 mb-3">
                <asp:Label ID="lblFirstName" runat="server" CssClass="form-label" Text="First Name"></asp:Label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter your first name"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                    ErrorMessage="First name is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                
            </div>
            <div class="col-md-6 mb-3">
                <asp:Label ID="lblLastName" runat="server" CssClass="form-label" Text="Last Name"></asp:Label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter your last name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                    ErrorMessage="Last name is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>

            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-6 mb-3">
                <asp:Label ID="lblEmail" runat="server" CssClass="form-label" Text="Email Address"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" TextMode="Email"></asp:TextBox>
               <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                    ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
               <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
                    ErrorMessage="Please enter a valid email address" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>


            </div>
            <div class="col-md-6 mb-3">
                <asp:Label ID="lblPhone" runat="server" CssClass="form-label" Text="Phone Number"></asp:Label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Enter your phone number"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone" 
                    ErrorMessage="Phone number is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>


            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-6 mb-3">
                <asp:Label ID="lblPassword" runat="server" CssClass="form-label" Text="Password"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Create a password" TextMode="Password"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" 
                    ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                
            </div>
            <div class="col-md-6 mb-3">
                <asp:Label ID="lblConfirmPassword" runat="server" CssClass="form-label" Text="Confirm Password"></asp:Label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="Confirm your password" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                    ErrorMessage="Please confirm your password" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                    ControlToCompare="txtPassword" ErrorMessage="Passwords do not match" CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>

            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <asp:Label ID="lblAddress" runat="server" CssClass="form-label" Text="Address"></asp:Label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter your address" TextMode="MultiLine" Rows="2"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress" 
                    ErrorMessage="Address is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>

            </div>
        </div>

        <!-- Customer Specific Fields (shown/hidden via JavaScript) -->
        <asp:Panel ID="pnlCustomerFields" runat="server">
            <div class="row mb-3">
                <div class="col-md-6 mb-3">
                    <asp:Label ID="lblPreferences" runat="server" CssClass="form-label" Text="Dietary Preferences"></asp:Label>
                    <asp:DropDownList ID="ddlPreferences" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Select Preference --" Value="" />
                        <asp:ListItem Text="No Preference" Value="None" />
                        <asp:ListItem Text="Vegetarian" Value="Vegetarian" />
                        <asp:ListItem Text="Vegan" Value="Vegan" />
                        <asp:ListItem Text="Non Vegetarian" Value="Non Vegetarian" />
                        <asp:ListItem Text="Keto" Value="Keto" />
                        <asp:ListItem Text="Paleo" Value="Paleo" />
                    </asp:DropDownList>
                </div>                
               <div class="col-12 text-center mt-2">
    <asp:Label ID="lblCustomerStatus" runat="server" CssClass="fw-bold" ForeColor="Green" />
</div>

            </div>
        </asp:Panel>

        <!-- Supplier Specific Fields (shown/hidden via JavaScript) -->
        <asp:Panel ID="pnlSupplierFields" runat="server" Visible="false">
            <div class="row mb-3">
                <div class="col-md-6 mb-3">
                    <asp:Label ID="lblBusinessName" runat="server" CssClass="form-label" Text="Business Name"></asp:Label>
                    <asp:TextBox ID="txtBusinessName" runat="server" CssClass="form-control" placeholder="Enter your business name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBusinessName" runat="server" ControlToValidate="txtBusinessName" 
                        ErrorMessage="Business name is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>

                </div>
                <div class="col-md-6 mb-3">
                    <asp:Label ID="lblBusinessType" runat="server" CssClass="form-label" Text="Business Type"></asp:Label>
                    <asp:DropDownList ID="ddlBusinessType" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Select Type --" Value="" />
                        <asp:ListItem Text="Farmer" Value="Farmer" />
                        <asp:ListItem Text="Baked Goods" Value="Baked Goods" />
                        <asp:ListItem Text="Dairy Products" Value="Dairy Products" />
                        <asp:ListItem Text="Handmade Crafts" Value="Handmade Crafts" />
                        <asp:ListItem Text="Food Producer" Value="Food Producer" />
                        <asp:ListItem Text="Wholesaler" Value="Wholesaler" />
                        <asp:ListItem Text="Other" Value="Other" />
                    </asp:DropDownList>
                     <asp:RequiredFieldValidator ID="rfvBusinessType" runat="server" ControlToValidate="ddlBusinessType" 
                        ErrorMessage="Business type is required" CssClass="text-danger" Display="Dynamic" InitialValue=""></asp:RequiredFieldValidator>
                   
                </div>

            </div>
            <div class="col-12 text-center mt-2">
    <asp:Label ID="lblSupplierStatus" runat="server" CssClass="fw-bold" ForeColor="Green" />
</div>

        
        </asp:Panel>

        <div class="row mt-4">
            <div class="col-12">
                <div class="form-check mb-3">
                    <asp:CheckBox ID="chkTerms" runat="server" CssClass="form-check-input" />
                    <asp:Label ID="lblTerms" runat="server" CssClass="form-check-label" Text="I agree to the Terms of Service and Privacy Policy" AssociatedControlID="chkTerms"></asp:Label>
                    <asp:CustomValidator ID="cvTerms" runat="server" ErrorMessage="You must agree to the terms and conditions" 
                        ClientValidationFunction="ValidateTerms" CssClass="text-danger d-block" Display="Dynamic"></asp:CustomValidator>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-12 text-center">
                <asp:Button ID="btnRegister" runat="server" Text="Create Account" CssClass="btn btn-primary register-btn" OnClick="btnRegister_Click" />
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-12 text-center">
                <p>Already have an account? <a href="Login.aspx" class="text-primary">Login here</a></p>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function ValidateTerms(source, args) {
            args.IsValid = document.getElementById('<%= chkTerms.ClientID %>').checked;
        }
    </script>


</asp:Content>