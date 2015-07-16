function updateLevelRows() {
    toggleLevelRows("Error");
    toggleLevelRows("Warning");
    toggleLevelRows("Information");
}

function setLevelFilterButtonStyle() {
    var error = getCookie("ErrorFilterButton");
    var warning = getCookie("WarningFilterButton");
    var information = getCookie("InformationFilterButton");

    if (error == "Checked") {
        document.getElementById("ErrorFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("ErrorFilterButton").style.border = "1px solid #000";
    }

    if (warning == "Checked") {
        document.getElementById("WarningFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("WarningFilterButton").style.border = "1px solid #000";
    }

    if (information == "Checked") {
        document.getElementById("InformationFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("InformationFilterButton").style.border = "1px solid #000";
    }
}

function toggleLevelFilterButton(level) {
    if (document.getElementById(level + "FilterButton").style.border == "4px solid rgb(255, 255, 255)") {
        document.getElementById(level + "FilterButton").style.border = "1px solid #000";
        setCookie(level + "FilterButton", "UnChecked", 3);
    } else {
        document.getElementById(level + "FilterButton").style.border = "4px solid #fff";
        setCookie(level + "FilterButton", "Checked", 3);
    }

    toggleLevelRows(level);
}

function toggleLevelRows(level) {
    var rows;

    if (level == "Error") {
        var error = getCookie("ErrorFilterButton");
        rows = document.getElementsByClassName("Error");
        for(var i = 0; i < rows.length; i++) {
            if (error == "Checked") {
                //alert(rows[i].parentNode.parentNode);
                rows[i].parentNode.parentNode.style.display = "none";
            } else {
                //alert(rows[i].parentNode.parentNode);
                rows[i].parentNode.parentNode.style.display = "table-row";
            }
        }
    }
    if (level == "Warning") {
        var warning = getCookie("WarningFilterButton");
        rows = document.getElementsByClassName("Warning");
        for (var i = 0; i < rows.length; i++) {
            if (warning == "Checked") {
                rows[i].parentNode.parentNode.style.display = "none";
            } else {
                rows[i].parentNode.parentNode.style.display = "table-row";
            }
        }
    }
    if (level == "Information") {
        var information = getCookie("InformationFilterButton");
        rows = document.getElementsByClassName("Information");
        for (var i = 0; i < rows.length; i++) {
            if (information == "Checked") {
                rows[i].parentNode.parentNode.style.display = "none";
            } else {
                rows[i].parentNode.parentNode.style.display = "table-row";
            }
        }
    }
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}