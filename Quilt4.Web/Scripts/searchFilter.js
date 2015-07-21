//Common
function updateRows() {
    var parameters = window.location.href.split("?").pop();
    var keys = parameters.split("&");
    keys.shift();//remove searhText
    console.log(keys);

    //Show row ONLY if all keys is set to show
    //If no keys, show all

    var rows = document.getElementById("resultlist").getElementsByTagName("tr");
    //console.log(rows);

    if (keys.length > 0) {
        for (var i = 1; i < rows.length; i++) {
            var showcount = 0;
            var childs = rows[i].getElementsByTagName("td");
            console.log("Childs: " + childs.length);
            for (var j = 0; j < childs.length; j++) {
                //console.log(childs[j].className + ": " + getUrl(childs[j].className));
                if (getUrl(childs[j].className) == "Show") {
                    showcount++;
                }
            }
            //console.log("----------------------------");

            if (keys.length == showcount) {
                rows[i].style.display = "table-row";
            } else {
                rows[i].style.display = "none";
            }
        }
    } else {
        for (var a = 1; a < rows.length; a++) {
            rows[a].style.display = "table-row";
        }
    }
}

//LEVELFILTER
function setLevelFilterButtonStyle() {
    var error = getUrl("ErrorFilterButton");
    var warning = getUrl("WarningFilterButton");
    var information = getUrl("InformationFilterButton");

    if (error == "Show") {
        document.getElementById("ErrorFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("ErrorFilterButton").style.border = "1px solid #000";
    }

    if (warning == "Show") {
        document.getElementById("WarningFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("WarningFilterButton").style.border = "1px solid #000";
    }

    if (information == "Show") {
        document.getElementById("InformationFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("InformationFilterButton").style.border = "1px solid #000";
    }

}

function toggleLevelFilterButton(level) {
    if (document.getElementById(level + "FilterButton").style.border == "4px solid rgb(255, 255, 255)") {
        document.getElementById(level + "FilterButton").style.border = "1px solid #000";
        setUrl(level + "FilterButton", "Hide");
    } else {
        document.getElementById(level + "FilterButton").style.border = "4px solid #fff";
        setUrl(level + "FilterButton", "Show");
    }

    updateRows();
}

//VERSIONFILTER
function toggleVersionFilterButton(version) {
    var url = getUrl("Version-" + version);

    if (url == "Show") {
        document.getElementById("Initiative-" + version).style.padding = "5px";
    }
}

function toggleVersionFilterButton(version) {
    if (document.getElementById(version).style.padding == "10px") {
        document.getElementById(version).style.padding = "5px";
        setUrl(version, "Show");
    } else {
        document.getElementById(version).style.padding = "10px";
        setUrl(version, "Hide");
    }

    updateRows();
}
//APPLICATIONFILTER
function toggleApplicationFilterButton(application) {
    var url = getUrl("Application-" + application);

    if (url == "Show") {
        document.getElementById("Initiative-" + application).style.padding = "5px";
    }
}

function toggleApplicationFilterButton(application) {
    if (document.getElementById(application).style.padding == "10px") {
        document.getElementById(application).style.padding = "5px";
        setUrl(application, "Show");
    } else {
        document.getElementById(application).style.padding = "10px";
        setUrl(application, "Hide");
    }

    updateRows();
}

//INITIATIVEFILTER
function setInitiativeFilterButtonStyle(initiativeName) {
    var url = getUrl("Initiative-" + initiativeName);
    
    if (url == "Show") {
        document.getElementById("Initiative-" + initiativeName).style.padding = "5px";
    } //else {
    //    document.getElementById("Initiative-" + initiativeName).style.padding = "10px";
    //}
}

function toggleInitiativeFilterButton(initiativeName) {
    if (document.getElementById(initiativeName).style.padding == "10px") {
        document.getElementById(initiativeName).style.padding = "5px";
        setUrl(initiativeName, "Show");
    } else {
        document.getElementById(initiativeName).style.padding = "10px";
        setUrl(initiativeName, "Hide");
    }

    updateRows();
}

//URL
function setUrl(name, value) {
    var parameters = window.location.href.split("?").pop();
    var keys = parameters.split("&");

    for (var i = 0; i < keys.length; i++) {
        if (keys[i].indexOf(name.replace("Button", "")) >= 0) {
            window.history.replaceState("", "", window.location.href.replace("&" + keys[i], ""));
            return;
        }
    }
    window.history.replaceState("", "", window.location.href + "&" + name + "=" + value);
}

function getUrl(name) {
    var parameters = window.location.href.split("?").pop();
    var keys = parameters.split("&");

    for (var i = 0; i < keys.length; i++) {
        if (keys[i].indexOf(name.replace("Button", "")) >= 0) {
            return keys[i].split("=").pop();
        }
    }
    return "";
}