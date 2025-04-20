<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="UrbanFoodWeb.Supplier.Category" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .btn-icon {
            padding: 0.25rem 0.5rem;
            margin: 0 2px;
        }
        .image-preview {
            border: 1px solid #ddd;
            padding: 5px;
            border-radius: 4px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Category Management</h4>
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtCategoryName">Category Name</label>
                                    <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control" placeholder="Enter category name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server" 
                                        ControlToValidate="txtCategoryName" ForeColor="Red" 
                                        ErrorMessage="Category name is required" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <label for="txtDescription">Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                        TextMode="MultiLine" Rows="3" placeholder="Enter category description"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="fileImage">Category Image</label>
                                    <div class="input-group">
                                        <div class="custom-file">
                                            <asp:FileUpload ID="fileImage" runat="server" CssClass="custom-file-input" />
                                            <label class="custom-file-label">Choose file</label>
                                        </div>
                                    </div>
                                    <asp:RequiredFieldValidator ID="rfvImage" runat="server" 
                                        ControlToValidate="fileImage" ForeColor="Red" ValidationGroup="NewCategory"
                                        ErrorMessage="Category image is required" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <small class="form-text text-muted">Recommended size: 300x300px. Max file size: 2MB.</small>
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Active" Checked="true" />
                                </div>
                                <div class="form-group">
                                    <asp:HiddenField ID="hdnCategoryId" runat="server" Value="0" />
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" ValidationGroup="NewCategory" OnClick="btnSave_Click" />
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success" OnClick="btnUpdate_Click" Visible="false" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" CausesValidation="false" />
                                </div>
                                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info" Visible="false"></asp:Label>
                            </div>
                            <div class="col-md-6">
                                <div id="imgPreview" runat="server" class="mb-3 image-preview" visible="false">
                                    <label>Current Image:</label>
                                    <div>
                                        <asp:Image ID="imgCategory" runat="server" CssClass="img-thumbnail" Width="200" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Category List</h4>
                        <div class="table-responsive">
                            <asp:GridView ID="gvCategories" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="False" OnRowCommand="gvCategories_RowCommand"
                                DataKeyNames="CategoryId" OnRowDeleting="gvCategories_RowDeleting">
                                <Columns>
                                    <asp:BoundField DataField="CategoryId" HeaderText="ID" />
                                    <asp:BoundField DataField="CategoryName" HeaderText="Category Name" />
                                    <asp:BoundField DataField="Description" HeaderText="Description" />
                                    <asp:TemplateField HeaderText="Image">
                                        <ItemTemplate>
                                            <asp:Image ID="imgCategoryList" runat="server" ImageUrl='<%# Eval("ImagePath") %>' 
                                                Width="80" Height="80" CssClass="rounded" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   <asp:TemplateField HeaderText="Active">
    <ItemTemplate>
        <asp:CheckBox ID="chkActive" runat="server" Enabled="false" 
            Checked='<%# Convert.ToBoolean(Convert.ToInt32(Eval("IsActive"))) %>' />
    </ItemTemplate>
</asp:TemplateField>
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditCategory" 
                                                CommandArgument='<%# Eval("CategoryId") %>' CssClass="btn btn-sm btn-info btn-icon">
                                                <i class="fa fa-edit"></i> Edit
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" 
                                                CommandArgument='<%# Eval("CategoryId") %>' CssClass="btn btn-sm btn-danger btn-icon"
                                                OnClientClick="return confirm('Are you sure you want to delete this category?');">
                                                <i class="fa fa-trash"></i> Delete
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center p-3">No categories found.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>