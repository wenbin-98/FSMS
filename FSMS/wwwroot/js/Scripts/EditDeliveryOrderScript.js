/* Use for Product Listing */
var Stocks = null;
/* Use for collecting form data before submit */
var formData = null;
/* Use for check the form whether is changed by user */
var currentFormData = null;
var documentStatus = 0;

var Merchant = null,
    Customer = null,
    DocumentInformation = null,
    Product = null,
    FormSetup = null;

const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 5000
});

const _Merchant = () => {
    var modal = $('#modal-edit-personal');
    var form = $('#form-edit-merchant');
    var submitButton = document.getElementById('btn-edit-merchant');
    var tempdata = null;

    //  Merchant core function
    const doMerchant = () => {
        // Compile data and submit to server for edit
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return false;
        }

        var dto = toDTO(form);
        var submitButton = document.getElementById('btn-edit-merchant');
        submitButton.setAttribute("disabled", '');

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: dto,
            dataType: "json",
            success: function (response) {
                $.ajax({
                    url: '/Customer/RenderCustomerDetailPartialView/' + dto["Merchant.Id"],
                    type: "GET",
                    success: function (response) {
                        $('#merchant-detail').html(response);
                    },
                    error: function (response) {
                        Toast.fire({
                            icon: 'error',
                            title: 'Error'
                        })
                    },
                    complete: function () {

                    }
                })
                modal.modal('hide');
                Toast.fire({
                    icon: 'success',
                    title: 'Information updated successfully.'
                })
            },
            error: function (response) {
                Toast.fire({
                    icon: 'error',
                    title: 'Information updated failed.'
                })
            },
            complete: function () {
                submitButton.removeAttribute("disabled");
            }
        })
    }

    // Get initial data before edit Merchant
    const initTempData = () => {
        tempdata = toDTO(form);
    }

    // Reset form to initial value if user abort the edit
    const resetForm = () => {
        fillDTOtoForm(tempdata);
    }

    // Assign all event will use when edit Merchant
    const wireUpEvent = () => {
        submitButton.addEventListener("click", doMerchant);

        $('#modal-edit-personal').on('show.bs.modal', initTempData);

        let modal = document.getElementById("modal-edit-personal")
        let closeButtons = modal.getElementsByClassName("close-btn");
        for (var i = 0; i < closeButtons.length; i++) {
            let closeButton = closeButtons[i];
            closeButton.addEventListener("click", resetForm);
        }
    }

    // initiate the function
    const init = () => {
        wireUpEvent();
    }

    init();
}

const _Customer = () => {
    var modal = $('#modal-edit-customer');
    var form = $('#form-edit-customer');
    var submitButton = document.getElementById('btn-edit-customer');
    var tempdata = null;

    const bindDropdown = () => {
        $('#ViewModel_CustomerId').select2({
            width: 'resolve',
            ajax: {
                url: "/Customer/GetCustomerDropDownData",
                dataType: 'json',
                data: function (params) {
                    return {
                        q: params.term, // search termi
                        page: params.page,
                        id: parseInt(customerId)
                    };
                },
                processResults: function (data, params) {
                    // parse the results into the format expected by Select2
                    // since we are using custom formatting functions we do not need to
                    // alter the remote JSON data, except to indicate that infinite
                    // scrolling can be used
                    params.page = params.page || 1;

                    return {
                        results: data,
                        pagination: {
                            more: (params.page * 30) < data.total_count
                        }
                    };
                },
                cache: true
            }
        });

        $('#ViewModel_CustomerId').on('select2:select', function (e) {
            let data = e.params.data;
            customerId = data.id;

            document.getElementById("Invoice_CustomerId").value = data.id;
        });
    }

    const doCustomer = () => {

        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return false;
        }

        submitButton.setAttribute("disabled", '');

        $.ajax({
            url: '/Customer/RenderCustomerDetailPartialView/' + customerId,
            type: "GET",
            success: function (response) {
                $('#customer-detail').html(response);
                modal.modal('hide');
                Toast.fire({
                    icon: 'success',
                    title: 'Customer changed successfully.'
                })
            },
            error: function (response) {
                Toast.fire({
                    icon: 'error',
                    title: 'Error'
                })
            },
            complete: function () {
                submitButton.removeAttribute("disabled");
            }
        })
    }

    const resetForm = () => {
        document.getElementById("Invoice_CustomerId").value = tempdata;
        $("#ViewModel_CustomerId").val('').trigger('change');
        customerId = tempdata;
    }

    const initTempData = () => {
        tempdata = document.getElementById("Invoice_CustomerId").value;
    }

    const wireUpEvent = () => {

        // Wire up dropdown
        bindDropdown();

        // Wire up submit button
        submitButton.addEventListener("click", doCustomer);

        // Wire up initiate initial data
        $('#modal-edit-customer').on('show.bs.modal', initTempData);

        // Wire up reset form event when close form modal
        let modal = document.getElementById("modal-edit-customer")
        let closeButtons = modal.getElementsByClassName("close-btn");
        for (var i = 0; i < closeButtons.length; i++) {
            let closeButton = closeButtons[i];
            closeButton.addEventListener("click", resetForm);
        }
    }

    const init = () => {
        wireUpEvent();
    }

    init();
}

const _DocumentInformation = () => {

    var modal = $('#modal-edit-information');
    var form = $('#form-edit-information');
    var submitButton = document.getElementById('btn-edit-information');
    var tempdata = null;

    const padLeadingZeros = (num, size) => {
        var s = num + "";
        while (s.length < size) s = "0" + s;
        return s;
    }

    const doDocument = () => {
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return false;
        }

        submitButton.setAttribute("disabled", '');

        let docNo = parseInt(document.getElementById("ViewModel_SerialNo").value);
        let date = document.getElementById("ViewModel_Date").value;
        let puchaseOrder = document.getElementById("ViewModel_PurchaseOrder").value;

        document.getElementById("doc-no").textContent = padLeadingZeros(docNo, 5);
        document.getElementById("doc-date").textContent = date;
        document.getElementById("purchase-order-no").textContent = puchaseOrder;

        document.getElementById("DeliveryOrder_SerialNo").value = docNo;
        document.getElementById("DeliveryOrder_Date").value = date;
        document.getElementById("DeliveryOrder_PurchaseOrder").value = puchaseOrder;

        submitButton.removeAttribute("disabled");

        modal.modal('hide');

        Toast.fire({
            icon: 'success',
            title: 'Information updated successfully.'
        })
    }

    const initTempData = () => {
        tempdata = toDTO(form);
    }

    const resetForm = () => {
        fillDTOtoForm(tempdata);
    }

    const wireUpEvent = () => {

        // Wire up submit button
        submitButton.addEventListener("click", doDocument);

        // Wire up initiate initial data
        $('#modal-edit-information').on('show.bs.modal', initTempData);

        // Wire up reset form event when close form modal
        let modal = document.getElementById("modal-edit-information")
        let closeButtons = modal.getElementsByClassName("close-btn");
        for (var i = 0; i < closeButtons.length; i++) {
            let closeButton = closeButtons[i];
            closeButton.addEventListener("click", resetForm);
        }

        // Wire up datepicker in modal
        $('#modal-edit-information .date-input').datepicker({
            format: 'dd-mm-yyyy',
            weekStart: 0,
            zIndex: 1100,
        });
    }

    const init = () => {
        wireUpEvent();
    }

    init();
}

const _Product = () => {
    var submitButton = document.getElementById("btn-add-product");
    var form = $('#form-add-product');
    var modal = $('#modal-add-product');

    const doProduct = () => {
        if (!form.valid()) {
            $('.input-validation-error').first().focus();
            return false;
        }

        unbindProductAttr();
        submitButton.setAttribute("disabled", '');

        let id = document.getElementById("AddProduct_Name").value;
        let quantity = document.getElementById("AddProduct_Quantity").value;
        let object = {};
        object.Id = parseInt(id);
        object.Quantity = parseInt(quantity);
        Stocks.push(object);

        $.ajax({
            url: '../Stocks/RenderDeliveryOrderProductPartialView',
            type: "POST",
            data: JSON.stringify({ "models": Stocks }),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                $('#product-container').html(response);
                compileProduct();
                modal.modal('hide');
                Toast.fire({
                    icon: 'success',
                    title: 'Product added successfully.'
                })
            },
            error: function (response) {
                Toast.fire({
                    icon: 'error',
                    title: 'Error!!'
                })
            },
            complete: function () {
                resetForm();
                bindProductAttr();
                submitButton.removeAttribute("disabled");
            }
        })
    }

    const resetForm = () => {
        let form = document.getElementById("form-add-product");
        form.reset();
        $("#AddProduct_Id").val('').trigger('change');
    }

    const compileProduct = () => {
        Stocks = [];
        let productContainer = document.getElementsByClassName("product-container")[0];
        let productRow = productContainer.getElementsByClassName("product-row");
        for (var i = 0; i < productRow.length; i++) {
            let object = {};
            let row = productRow[i];
            object.Id = parseInt(row.getElementsByClassName("product-id")[0].innerText);
            object.Quantity = parseInt(row.getElementsByClassName("product-quantity")[0].value);
            Stocks.push(object);
        }
        let stockInput = document.getElementById("DeliveryOrder_Stocks");

        console.log(Stocks.length);
        (Stocks.length !== 0 ? stockInput.value = 1 : stockInput.value = "")
    }

    const deleteAlert = (button) => {
        let data = button.dataset;
        Swal.fire({
            title: '<strong><u>Attention</u></strong>',
            icon: 'warning',
            html:
                'Are you sure to delete this product? <br />' +
                data.name + ' (' + data.desc + ')',
            showCancelButton: true,
            focusConfirm: false,
            confirmButtonText:
                'Yes',
            cancelButtonText:
                'Cancel',
        }).then((data => {
            if (data.value === true) {
                removeProduct(button);
            }
        }))
    }

    const bindProductAttr = () => {
        var container = document.getElementById("product-container");
        var removeProductButtons = container.getElementsByClassName("btn-danger");
        for (var i = 0; i < removeProductButtons.length; i++) {
            var button = removeProductButtons[i];
            button.addEventListener('click', alertBeforeDelete);
        }
    }

    const unbindProductAttr = () => {
        var container = document.getElementById("product-container");
        var removeProductButtons = container.getElementsByClassName("btn-danger");
        for (var i = 0; i < removeProductButtons.length; i++) {
            var button = removeProductButtons[i];
            button.removeEventListener('click', alertBeforeDelete);
        }
    }

    const removeProduct = (button) => {
        let buttonClicked = button;
        buttonClicked.parentElement.parentElement.remove();
        compileProduct();
        documentStatus = 1;
    }

    const alertBeforeDelete = (event) => {
        let buttonClicked = event.target;
        deleteAlert(buttonClicked);
    }

    const wireUpEvent = () => {

        // Assign calculation and remove product event
        bindProductAttr();

        // Assign add product dropdown
        $('#AddProduct_Id').select2({
            width: 'resolve',
            ajax: {
                url: "../Stocks/GetStockDropDownData",
                dataType: 'json',
                data: function (params) {
                    return {
                        q: params.term, // search term
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    // parse the results into the format expected by Select2
                    // since we are using custom formatting functions we do not need to
                    // alter the remote JSON data, except to indicate that infinite
                    // scrolling can be used
                    params.page = params.page || 1;

                    return {
                        results: data,
                        pagination: {
                            more: (params.page * 30) < data.total_count
                        }
                    };
                },
                cache: true
            }
        });

        // Assign add product dropdown option
        $('#AddProduct_Id').on('select2:select', function (e) {
            let data = e.params.data;
            $('#AddProduct_Quantity').val(data.quantity);
            $('#AddProduct_Description').val(data.description);
            $('#AddProduct_Name').val(data.id);
        });

        // Wire up reset form event when close form modal
        let modal = document.getElementById("modal-add-product")
        let closeButtons = modal.getElementsByClassName("close-btn");
        for (var i = 0; i < closeButtons.length; i++) {
            let closeButton = closeButtons[i];
            closeButton.addEventListener("click", resetForm);
        }

        submitButton.addEventListener("click", doProduct);
    }

    const init = () => {
        wireUpEvent();
        compileProduct();
    }

    init();
}

const _formSetup = () => {
    var form = $("#form-edit-document");
    var submitButton = document.getElementById('btn-submit-form');

    const submitAlert = () => {
        var form = $("#form-edit-document");
        if (!form.valid()) {
            document.body.scrollTop = 0;
            document.documentElement.scrollTop = 0;
            return false;
        }
        Swal.fire({
            title: '<strong><u>Attention</u></strong>',
            icon: 'warning',
            html:
                'Please make sure all information is correct. <br />' +
                'All changes cannot be reverted',
            showCancelButton: true,
            focusConfirm: false,
            confirmButtonText:
                'Yes',
            cancelButtonText:
                'Cancel',
        }).then((data => {
            if (data.value === true) {
                submitData();
            }
        }))
    }

    const submitData = () => {
        if (!form.valid()) {
            document.body.scrollTop = 0;
            document.documentElement.scrollTop = 0;
            return false;
        }

        submitButton.setAttribute("disabled", '');
        formData = toDTO(form);
        let token = formData["__RequestVerificationToken"];

        formData["DeliveryOrder.Stocks"] = Stocks;

        formData = formatObjectKey(formData);
        formData["Date"] = moment(formData["Date"], 'DD/MM/YYYY');
        let link = "./EditDeliveryOrder";

        $.ajax({
            url: link,
            type: "POST",
            headers: { 'RequestVerificationToken': token },
            data: JSON.stringify(formData),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                window.removeEventListener("beforeunload", detectChanges);
                window.location.href = ("./Details/" + documentId);
            },
            error: function (response) {
                Toast.fire({
                    icon: 'error',
                    title: 'Error!!'
                })
            },
            complete: function () {
                submitButton.removeAttribute("disabled");
            }
        })
    }

    const assignValue = () => {
        document.getElementById("DeliveryOrder_Id").value = documentId;
        document.getElementById("DeliveryOrder_SerialNo").value = document.getElementById("ViewModel_SerialNo").value;
        document.getElementById("DeliveryOrder_CustomerId").value = customerId;
        document.getElementById("DeliveryOrder_Date").value = document.getElementById("ViewModel_Date").value;
        document.getElementById("DeliveryOrder_PurchaseOrder").value = document.getElementById("ViewModel_PurchaseOrder").value;
    }

    const detectChanges = (e) => {
        let form = document.getElementById("form-edit-document");
        formData = toDTO(form);
        if ((JSON.stringify(currentFormData) != JSON.stringify(formData)) || documentStatus != 0) {
            e.preventDefault();
            e.returnValue = 'Changes you made may not be saved.';
        }
    }

    /* Used to save inital form data for comparison later */
    const initCurrentFormData = () => {
        let form = document.getElementById("form-edit-document");
        currentFormData = toDTO(form);
    }

    const wireUpEvent = () => {
        // Assign submit event
        submitButton.addEventListener("click", submitAlert);

        // Assign check document changes event
        window.addEventListener("beforeunload", detectChanges);
    }

    const init = () => {
        wireUpEvent();
        assignValue();
        initCurrentFormData();
    }

    init();
}

const init = () => {
    Customer = _Customer();
    Merchant = _Merchant();
    DocumentInformation = _DocumentInformation();
    Product = _Product();
    FormSetup = _formSetup();
}

const ready = (fn) => {
    if (document.readyState != 'loading') {
        fn();
    } else if (document.addEventListener) {
        document.addEventListener('DOMContentLoaded', fn);
    } else {
        document.attachEvent('onreadystatechange', function () {
            if (document.readyState != 'loading')
                fn();
        });
    }
}

ready(init);