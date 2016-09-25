/**
    Content of notifications
**/
var Notifications = {
    /* Group Name*/
    // RegularExpression
    INVALID_GROUPNAME: 'Group name must not contain \ / ? % * : | "" < >',
    // StringLength
    INVALID_GROUPNAME_STR: 'Group name must be in range 1 to 50 characters',
    // isGroupExists
    INVALID_GROUPNAME_EXISTS: 'Group name already exists',
    // Required
    INVALID_GROUPNAME_REQUIRED: 'Group name is required',
    // Is duplicate
    INVALID_GROUPNAME_DUPLICATE: 'Group name is duplicate',
    /*--edit user---*/
    //email exists
    INVALID_EMAIL_USED: 'This Email is used by another account!',

    /* User */
    INVALID_FIRST_LAST_NAME_REQUIRED: 'First and Last name are required',

    INVALID_ACCOUNT_REQUIRED: 'Account is required',

    INVALID_EMAIL_REQUIRED: 'Email is required',

    INVALID_PHONE_REQUIRED: 'Telephone number is required',

    INVALID_PASSWORD_REQUIRED: 'Password is required',

    INVALID_EMAIL: 'Please enter a valid email address',

    INVALID_PASSWORD_MIN_LENGTH: 'Password must contain at least 6 characters',

    INVALID_PASSWORD_MAX_LENGTH: 'Password contains only 30 characters',

    INVALID_PASSWORD_DIGIT: 'Password must contain at least 1 number',

    INVALID_PASSWORD_LOWERCASE: 'Password must contain at least 1 lowercase character (a-z)',

    INVALID_PASSWORD_UPPERCASE: 'Password must contain at least 1 uppercase character (A-Z)',

    INVALID_PASSWORD_SPECIAL_CHARACTERS: 'Password only allow - , _, @, ., /, #, &, + characters',

    INVALID_EMAIL_EXIST: 'Email already exists',

    INVALID_ACCOUNT_NAME: 'Your account name is not valid. Only characters A-Z, a-z and 0-9 are acceptable',

    INVALID_ACCOUNT_EXIST: 'Account name already exsits',

    INVALID_ACCOUNT_LENGTH: 'Account name must be in range 1 to 50 characters',

    INVALID_FIRSTNAME_LENGTH: 'First name must be in range 1 to 50 characters',

    INVALID_FIRSTNAME: 'First name is not valid. Only characters A-Z, a-z are acceptable',

    INVALID_PHONE: 'Please using a valid 10-digit phone number',

    INVALID_ACCOUNT_DUPLICATE: 'Duplicate account name',

    INVALID_EMAIL_DUPLICATE: 'Duplicate email',
};

/**
    Validate regular expression Group name
    @para: string groupName
    @return: bool
**/
function validateGroupName(groupName) {
    var reg = /^[0-9a-zA-Z ... _]+$/;
    if (!reg.test(groupName))
        return true;
    return false;
}

/**
    Validate length of Group name
    @para: string groupName
    @return: bool
**/
function validateGroupNameLength(groupName) {
    return !((groupName.length >= 1) && (groupName.length <= 50));
}

/**
    Check Group name is not exists?
    @para: string groupName
    @return: bool
**/
function validateGroupNameExists(groupName) {
    var res = "";
    $.ajax({
        url: '/Group/isExists?Name=' + groupName,
        type: 'GET',
        async: false,
        success: function (e) {
            res = e;
        }
    });
    return !res;
}

/**
    Method for handson table to validate Group name
    @para: groupname, callback
    @return: callback method
**/
function validateHSGroupName(groupName, callback) {
    setTimeout(function () {
        if (!groupName) {
            showModal(Notifications.INVALID_GROUPNAME_REQUIRED);
            callback(false);
        }
        else if (validateGroupName(groupName)) {
            showModal(Notifications.INVALID_GROUPNAME);
            callback(false);
        } else if (validateGroupNameLength(groupName)) {
            showModal(Notifications.INVALID_GROUPNAME_STR);
            callback(false);
        } else if (validateGroupNameExists(groupName)) {
            showModal(Notifications.INVALID_GROUPNAME_EXISTS);
            callback(false);
        } else {
            callback(true);
        }
    }, 200);
}

/**
    Method for validate email
    @para: string email
    @return: true false
**/
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return !re.test(email);
}


function showModal(msg) {
    toastr.options = {
        preventDuplicates:true    
    };
    toastr.error(msg, "Error");
}
function validateEmailExistence(Email) {

}

$(document).ready(function () {

    /*
        Email template
    */
    $('#btn-choosetemplate').click(function () {
        var id = $('#Id option:selected').val();
        window.location.href = '/EmailTemplates/Edit/' + id;
    })

    /*
        Add users view
    */

    // users array 
    var users = [];
    $('#btn-AddUser').click(function () {
        var rows = info.countRows();
        var emptyRows = info.countEmptyRows(true);
        var isDuplicate = false;

        // Validate Parent Group field
        var parentGroup = $('#text-parent-group-add-user-view').val();
        var isParentGroupNull;

        if (parentGroup == "") {
            $('#validate-parent-group-add-user-view').html("Parent Group is required");
            isParentGroupNull = true;
        }
        else {
            $('#validate-parent-group-add-user-view').html("");
            isParentGroupNull = false;
        }

        // If Save to LDAP checked
        var saveLDAPChecked;
        if (document.getElementById('saveToLdap').checked) {
            saveLDAPChecked = true;
        }
        else {
            saveLDAPChecked = false;
        }

        // Check duplicate input of both Account and Email fields
        var accounts = info.getDataAtCol(2);
        accounts.pop();     // not check the bottom row
        $.each(accounts, function (row, account) {
            if (account != null && account !== "") {
                var count = accounts.filter(function (a) { return a == account }).length;
                if (count > 1) {
                    info.setCellMeta(row, 2, 'valid', false);
                    showModal(Notifications.INVALID_ACCOUNT_DUPLICATE);
                    isDuplicate = true;
                }
            }
       });

        info.render();

        var emails = info.getDataAtCol(3);
        emails.pop();
        $.each(emails, function (row, email) {
            var count = emails.filter(function (e) { return e == email }).length;
            if (email != null && email !== "") {
                if (count > 1) {
                    info.setCellMeta(row, 3, 'valid', false);
                    showModal(Notifications.INVALID_EMAIL_DUPLICATE);
                    isDuplicate = true;
                }
            }
        });

        info.render();

        // Validate Handsontable
        if (rows - emptyRows > 0 && !isDuplicate) {
            info.validateCells(function (valid) {
                if (valid && !isParentGroupNull && saveLDAPChecked) {
                    // create users array
                    for (i = 0; i < rows - emptyRows; i++) {
                        var user = {
                            FirstName: info.getDataAtCell(i, 0),
                            LastName: info.getDataAtCell(i, 1),
                            Account: info.getDataAtCell(i, 2),
                            Email: info.getDataAtCell(i, 3),
                            Phone: info.getDataAtCell(i, 4),
                            Password: info.getDataAtCell(i, 5),
                            Roles: "User",
                            ParentDn: $('#text-parent-group-dn-add-user-view').val()
                        };
                        users.push(user);
                    }

                    // add users
                    $.ajax({
                        type: 'POST',
                        url: '/User/Add/',
                        traditional: true,
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({ users: users }),
                        success: function (url) {
                            users = [];
                            window.location.href = url;
                        },
                    })
                }
            });
        }
    });

    // Load Ajax Tree - using TreeView Boostrap

    $.ajax({
        url: '/Home/GetUserAndGroupDataJson',
        type: 'GET',
        success: function (data) {
            var tree = [];
            tree.push(data);

            var $searchableTree = $('#tree').treeview({
                showBorder: false,
                nodeIcon: 'fa fa-user',
                data: tree
            });

            var search = function (e) {
                var pattern = $('#input-search').val();
                var results = $searchableTree.treeview('search', pattern);
            }

            $('#input-search').on('keyup', search);

            $('#btn-search').on('click', search);
            $searchableTree.treeview('expandAll', { levels: 1 });

            $('#btn-refresh').on('click', function () {
                $searchableTree.treeview('expandAll', { levels: 1 });
            });

            $('#tree').on('nodeSelected', function (event, node) {
                // if choose group
                if (node.dn.substring(0, 2) == "ou") {

                    // Add User View
                    $('#text-parent-group-add-user-view').val(node.text);
                    $('#text-parent-group-dn-add-user-view').val(node.dn);
                    $('#validate-parent-group-add-user-view').html("");

                    // Add Group View
                    $('#ParentDn').val(node.dn);
                    $('#parentGroup').val(node.text);

                    // Move Group View
                    $('.nameGroupMove').val(node.text);
                    $('#Dn').val(node.dn);

                    var currentGroup = $('.nameGroupMove').val();
                    var moveTo = $('#selectedDn option:selected').text();

                    if (currentGroup == moveTo) {
                        $('#selectedDn').val('ou=People,dc=maxcrc,dc=com');
                    }

                    $('#selectedDn option').each(function (i, item) {
                        var itemValue = $(item).text();
                        var currentGroup = $('.nameGroupMove').val();

                        if (itemValue == currentGroup) {
                            item.disabled = true;
                            $(item).attr('style', 'color:#cccccc;');
                        }
                        else {
                            item.disabled = false;
                            $(item).removeAttr('style');
                        }
                    });

                    $('#selectedDn').prop("disabled", false);

                    // Edit Group View
                    //if (node.text == "AM" && node.parent == "") {
                    //    showModal(Notifications.D);
                    //}

                    $('#btn-edit').click(function () {
                        window.location.href = '/Group/Edit?Name=' + node.text;
                    });

                    // Delete Group
                    $('#btn-trash').click(function () {
                        if (node.dn.substring(0, 2) == "ou") {
                            $('#deleteGroupModal').modal('show');
                            $('#deleteUserModal').modal('hide');
                            $('#nameOfGroupDeleteting').text(node.text);
                        }
                    });

                    $('#btn-delete-group').click(function () {
                        var nameOfGroupDeleting = $('#nameOfGroupDeleteting').text();
                        window.location.href = '/Group/Delete?Name=' + nameOfGroupDeleting;
                    });

                    // Export group to excel
                    $('#btn-export').click(function () {
                        window.location.href = '/Group/ExportToExcel?Name=' + node.text;
                    });

                } else {
                    // Move Group View
                    $('#Account').val(node.text);
                    $('#Dn').val(node.dn);
                    $('#selectedDn option').each(function (i, item) {
                        var itemValue = $(item).text();
                        var str = node.dn;
                        var res = str.split(',');
                        var currentGroup = res[1].substring(3, res[1].length);
                        if (itemValue == currentGroup) {
                            item.disabled = true;
                            $(item).attr('style', 'color:#cccccc;');
                        }
                        else {
                            item.disabled = false;
                            $(item).removeAttr('style');
                        }
                    });

                    $('#selectedDn').prop("disabled", false);

                    // Delete User
                    $('#btn-trash').click(function () {
                        if (node.dn.substring(0, 3) == "uid") {
                            $('#deleteGroupModal').modal('hide');
                            $('#deleteUserModal').modal('show');
                            $('#nameOfUserDeleting').text(node.text);
                        }
                    });
                    $('#btn-delete-user').click(function () {
                        var nameOfUserDeleting = $('#nameOfUserDeleting').text();
                        window.location.href = '/User/Delete?Name=' + nameOfUserDeleting;
                    });

                    // Edit User
                    $('#btn-edit').click(function () {
                        window.location.href = '/User/EditUser?Name=' + node.text;

                    });
                }
            });
        }
    });

    /*
        Group
    */
    // Load Handsontable
    var dataElement = document.querySelector('#data');
    var dataElementContainer = dataElement.parentNode;
    var dataSettings = {
        startRows: 3,
        columns: [
            {
                data: 'groupname',
                validator: validateHSGroupName,
                allowInvalid: true
            }
        ],
        stretchH: 'all',
        minSpareRows: 1,
        autoWrapRow: true,
        rowHeaders: true,
        colHeaders: [
            'Group Name'
        ],
        manualRowResize: true,
        manualColumnResize: true,
    };
    var data = new Handsontable(dataElement, dataSettings);

    $("#btn-SaveManyGroups").click(function (e) {
        var lstGroupName = data.getDataAtCol(0); // get data from col Group Name, return Array<String>
        var checkGroupName = new Array(lstGroupName.length);
        var isValid = true;

        for (var i = 0; i < lstGroupName.length - 1; i++) {
            if (!lstGroupName[i]) {
                isValid = false;
                checkGroupName[i] = 1;
                data.setCellMeta(i, 0, 'valid', false);
                data.render();
                showModal(Notifications.INVALID_GROUPNAME_REQUIRED);
            }
            if (checkGroupName[i] == 1) continue;
            for (var j = 0; j < lstGroupName.length - 1; j++) {
                if (i == j) continue;
                else {
                    if (lstGroupName[i] == lstGroupName[j]) {
                        isValid = false;
                        checkGroupName[j] = 1;
                        showModal(Notifications.INVALID_GROUPNAME_DUPLICATE);
                        data.setCellMeta(j, 0, 'valid', false);
                        data.render();
                    }
                }
            }
        }
        if (isValid == false) return false; // has invalid
        else {
            var parentDn = $('#ParentDn').val();
            lstGroupName.pop();
            $.ajax({
                url: '/Group/AddManyGroups/',
                data: { lstGroupName: lstGroupName, parentDn: parentDn },
                type: 'POST'
            });
        }

    })

});
