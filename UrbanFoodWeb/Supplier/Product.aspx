<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="UrbanFoodWeb.Supplier.Product" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Product Management</h4>
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="ddlCategory">Category</label>
                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory" ForeColor="Red" InitialValue="0" ErrorMessage="Please select a category" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>

                                <!-- Add this near the top of your form, before your existing product form fields -->
<div class="form-group" id="productForm">
    <div class="row">
        <div class="col-md-6">
            <label for="ddlProductsToUpdate">Select Product to Update:</label>
            <asp:DropDownList ID="ddlProductsToUpdate" runat="server" CssClass="form-control" 
                AutoPostBack="true" OnSelectedIndexChanged="ddlProductsToUpdate_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
    </div>
</div>
<hr />
<!-- Your existing form fields continue here -->


                                <div class="form-group">
                                    <label for="txtProductName">Product Name</label>
                                    <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" placeholder="Enter product name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvProductName" runat="server" ControlToValidate="txtProductName" ForeColor="Red" ErrorMessage="Product name is required" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <label for="txtDescription">Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter product description"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ForeColor="Red" ErrorMessage="Description is required" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <label for="txtPrice">Price</label>
                                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="Enter product price"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ControlToValidate="txtPrice" ForeColor="Red" ErrorMessage="Price is required" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revPrice" runat="server" ControlToValidate="txtPrice" ForeColor="Red" ValidationExpression="^\d+(\.\d{1,2})?$" ErrorMessage="Please enter a valid price" Display="Dynamic"></asp:RegularExpressionValidator>
                                </div>
                                <div class="form-group">
                                    <label for="txtQuantity">Quantity</label>
                                    <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" placeholder="Enter available quantity"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ControlToValidate="txtQuantity" ForeColor="Red" ErrorMessage="Quantity is required" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revQuantity" runat="server" ControlToValidate="txtQuantity" ForeColor="Red" ValidationExpression="^[0-9]+$" ErrorMessage="Please enter a valid quantity" Display="Dynamic"></asp:RegularExpressionValidator>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="fileImage">Product Image</label>
                                    <div class="input-group">
                                        <div class="custom-file">
                                            <asp:FileUpload ID="fileImage" runat="server" CssClass="custom-file-input" />
                                            <label class="custom-file-label">Choose file</label>
                                        </div>
                                    </div>
                                    <asp:RequiredFieldValidator ID="rfvImage" runat="server" ControlToValidate="fileImage" ForeColor="Red" ErrorMessage="Product image is required" Display="Dynamic" ValidationGroup="AddProduct"></asp:RequiredFieldValidator>
                                    <small class="form-text text-muted">Recommended size: 500x500px. Max file size: 2MB.</small>
                                </div>

                                <asp:Panel ID="pnlCurrentImage" runat="server" CssClass="mb-3" Visible="false">
                                    <label>Current Image:</label>
                                    <div>
                                        <asp:Image ID="imgCurrent" runat="server" CssClass="img-thumbnail" Width="200" />
                                    </div>
                                </asp:Panel>

                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Active" Checked="true" />
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsFeatured" runat="server" Text="Featured Product" />
                                </div>
                                <div class="form-group">
                                    <asp:HiddenField ID="hdnProductId" runat="server" Value="0" />
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="AddProduct" />
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success" OnClick="btnUpdate_Click" Visible="false" ValidationGroup="AddProduct" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" CausesValidation="false" />
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label ID="lblCurrentImage" runat="server" CssClass="text-muted d-block" Visible="false" />
                                <asp:HiddenField ID="hfCurrentImageUrl" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Product Grid -->
        <div class="row mt-3">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Your Products</h4>
                        <div class="table-responsive">
                            <asp:GridView ID="gvProducts" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="False" OnRowCommand="gvProducts_RowCommand"
                                DataKeyNames="ProductID" EmptyDataText="No products found">
                                <Columns>
                                    <asp:BoundField DataField="ProductID" HeaderText="ID" />
                                    <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                    <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                    <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="QuantityAvailable" HeaderText="Quantity" />
                                    <asp:TemplateField HeaderText="Image">
                                        <ItemTemplate>
                                            <asp:Image ID="imgProductList" runat="server" ImageUrl='<%# Eval("ImageURL") %>' Width="80" Height="80" CssClass="rounded" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Active">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkIsActiveGrid" runat="server" Checked='<%# Convert.ToBoolean(Convert.ToInt32(Eval("IsActive"))) %>' Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Featured">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkIsFeaturedGrid" runat="server" Checked='<%# Convert.ToBoolean(Convert.ToInt32(Eval("IsFeatured"))) %>' Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:Button ID="btnEdit" runat="server" CommandName="EditProduct" CommandArgument='<%# Eval("ProductID") %>' CssClass="btn btn-sm btn-info" Text="Edit" />
                                            <asp:Button ID="btnDelete" runat="server" CommandName="DeleteProduct" CommandArgument='<%# Eval("ProductID") %>' CssClass="btn btn-sm btn-danger" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this product?');" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>