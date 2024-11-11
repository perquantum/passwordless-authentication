$(function () {
    var l = abp.localization.getResource("ProductService");
	var productService = window.passwordlessAuthentication.productService.products.product;

    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Products/CreateModal"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Products/EditModal"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            name: $("#NameFilter").val(),
			priceMin: $("#PriceFilterMin").val(),
			priceMax: $("#PriceFilterMax").val()
        };
    };

    var dataTable = $("#ProductsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(productService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('ProductService.Products.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('ProductService.Products.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    productService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l("DeletedSuccessfully"));
                                            dataTable.ajax.reloadEx();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "name" },
			{ data: "price" }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reloadEx();
    });

    editModal.onResult(function () {
        dataTable.ajax.reloadEx();
    });

    $('#AbpContentToolbar button[name=CreateProduct]').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

	$("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reloadEx();
    });

    $("#AdvancedFilterSectionToggler").click(function (e) {
        $("#AdvancedFilterSection").toggle();
        var iconCss = $("#AdvancedFilterSection").is(":visible") ? "fa ms-1 fa-angle-up" : "fa ms-1 fa-angle-down";
        $(this).find("i").attr("class", iconCss);
    });

    $('#AdvancedFilterSection').on('keypress', function (e) {
        if (e.which === 13) {
            dataTable.ajax.reloadEx();
        }
    });

    $('#AdvancedFilterSection select').change(function() {
        dataTable.ajax.reloadEx();
    });
});
