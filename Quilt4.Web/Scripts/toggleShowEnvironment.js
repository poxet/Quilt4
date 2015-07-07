function toggleShowEnvironment(environment) {
    var rows;

    if (environment == "Dev") {
        rows = document.getElementsByClassName("DevColor");
    } else if (environment == "Ci") {
        rows = document.getElementsByClassName("CiColor");
    } else if (environment == "Prod") {
        rows = document.getElementsByClassName("ProdColor");
    }

    for (var i = 0; i < rows.length; i++) {
        if (rows[i].parentNode.parentNode.style.display == "table-row") {
            rows[i].parentNode.parentNode.style.display = "none";

        } else {
            rows[i].parentNode.parentNode.style.display = "table-row";
        }
    }
}

function changeButtonStyle(id) {
    if (id == "DevLegendButton" || id == "CiLegendButton" || id == "ProdLegendButton") {

        if (document.getElementById(id).style.border == "4px solid rgb(255, 255, 255)") {
            document.getElementById(id).style.border = "1px solid #000";
        } else {
            document.getElementById(id).style.border = "4px solid #fff";
        }

    }
}