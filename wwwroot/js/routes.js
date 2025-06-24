let map;
let idOfEditedRoute = -1;

let departureMarker, destinationMarker, routeLayer;
let departureCoords, destinationCoords;

function initializeMap() {
    if (!map) {
        map = L.map("map").setView([52.0693, 19.4803], 6);
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            attribution: "© OpenStreetMap contributors",
        }).addTo(map);

        map.on("click", function (e) {
            if (!departureCoords) {
                setDeparture(e.latlng);
                console.log(e.latlng);
            } else if (!destinationCoords) {
                setDestination(e.latlng);
                calculateRoute();
            } else {
                clearMarkers();
                setDeparture(e.latlng);
            }
        });
    }
}

function setDeparture(latlng) {
    departureCoords = latlng;
    if (departureMarker) map.removeLayer(departureMarker);

    departureMarker = L.marker(latlng, {
        icon: L.icon({
            iconUrl:
                "https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png",
            iconSize: [25, 41],
            iconAnchor: [12, 41],
        }),
    }).addTo(map);

    reverseGeocode(latlng, "form-departure");
}

function setDestination(latlng) {
    console.log(latlng);
    destinationCoords = latlng;
    if (destinationMarker) map.removeLayer(destinationMarker);

    destinationMarker = L.marker(latlng, {
        icon: L.icon({
            iconUrl:
                "https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png",
            iconSize: [25, 41],
            iconAnchor: [12, 41],
        }),
    }).addTo(map);

    reverseGeocode(latlng, "form-destination");
}

async function reverseGeocode(latlng, type) {
    try {
        const response = await fetch(
            `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latlng.lat}&lon=${latlng.lng}`
        );
        const data = await response.json();
        document.getElementById(type).value = data.display_name;
    } catch (error) {
        console.error("Error occured whilre  geocoding:", error);
    }
}

async function calculateRoute() {
    if (!departureCoords || !destinationCoords) {
        alert("Choose two points on the map");
        return;
    }

    try {
        const response = await fetch(
            `https://router.project-osrm.org/route/v1/driving/${departureCoords.lng},${departureCoords.lat};${destinationCoords.lng},${destinationCoords.lat}?overview=full&geometries=geojson`
        );

        const data = await response.json();

        if (data.routes && data.routes.length > 0) {
            const route = data.routes[0];
            console.log(data.routes);
            const distance = (route.distance / 1000).toFixed(2);
            const duration = Math.round(route.duration / 60);

            document.getElementById(
                "form-distance"
            ).value = `${distance} km (${duration} min - estimated)`;

            // Rysowanie trasy na mapie
            if (routeLayer) map.removeLayer(routeLayer);
            routeLayer = L.geoJSON(route.geometry, {
                style: { color: "blue", weight: 5, opacity: 0.7 },
            }).addTo(map);
        }
    } catch (error) {
        alert(
            "An error occurred while calculating the route: " + error.message
        );
    }
}

async function getAvaliableDriversAndVehicles() {
    console.log();

    const data = fetch("/page/routes/avaliable-drivers-and-vehicles")
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error: " + response.status);
            }
            return response.json();
        })
        .then((data) => {
            const drivers = data.avaliableDrivers;
            const vehicles = data.avaliableVehicles;

            console.log("Kierowcy:", drivers);
            console.log("Pojazdy:", vehicles);

            data.avaliableDrivers.forEach((element) => {
                addSelectOption(
                    "form-assignedDriver",
                    element["id"],
                    element["name"] + " " + element["lastName"]
                );
            });

            data.avaliableVehicles.forEach((element) => {
                addSelectOption(
                    "form-assignedVehicle",
                    element["id"],
                    element["licensePlate"] +
                        " - " +
                        element["brand"] +
                        " - " +
                        element["model"]
                );
            });
        })
        .catch((error) => {
            console.log("Error on post: ", error);

            return error;
        });

    console.log("Arrays: ", data);
}

function clearMarkers() {
    if (departureMarker) map.removeLayer(departureMarker);
    if (destinationMarker) map.removeLayer(destinationMarker);
    if (routeLayer) map.removeLayer(routeLayer);

    departureCoords = null;
    destinationCoords = null;

    document.getElementById("form-distance").value = "";
}

document
    .getElementById("route-form")
    .addEventListener("submit", async function (e) {
        e.preventDefault();

        try {
            const formData = new FormData(this);
            const jsonData = Object.fromEntries(formData);

            var distanceFloat = parseFloat(jsonData["distance"].split(" ")[0]);
            jsonData["distance"] = distanceFloat;

            jsonData["departureCoords"] =
                departureCoords.lat + ", " + departureCoords.lng;
            jsonData["destinationCoords"] =
                destinationCoords.lat + ", " + destinationCoords.lng;

            idOfEditedRoute != -1
                ? await updateRoute(jsonData)
                : await addRoute(jsonData);

            console.log("Submit route triggered: ", jsonData);
            this.reset();
            changeView(true);
        } catch (error) {
            console.error("Error:", error);
        }
    });

document.addEventListener("DOMContentLoaded", function () {
    document
        .getElementById("routes-table")
        .addEventListener("click", function (e) {
            if (e.target.classList.contains("delete-btn")) {
                deleteRoute(e);
            }
            if (e.target.classList.contains("edit-btn")) {
                editRoute(e);
            }
        });
});

async function editRoute(e) {
    idOfEditedRoute =
        e.target.parentElement.querySelector("td.route-id").textContent;

    fetch("/api/routes/" + idOfEditedRoute)
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error: " + response.status);
            }
            return response.json();
        })
        .then((data) => {
            console.log("Route data:", data);

            document.getElementById("form-departure").value =
                data.departure || "";
            document.getElementById("form-destination").value =
                data.destination || "";

            setCurrentDriverAndvehicle(
                data.assignedDriver,
                data.assignedVehicle
            );

            changeView(false);

            document.getElementById("form-distance").value =
                data.distance + " km" || "";

            if (data.departureCoords != null) {
                let [lat, lng] = data.departureCoords
                    .split(", ")
                    .map((coord) => parseFloat(coord.trim()));

                let latlng = L.latLng(lat, lng);

                console.log("Departure: ", lat, ", ", lng);
                setDeparture(latlng);
            }

            if (data.destinationCoords != null) {
                [lat, lng] = data.destinationCoords
                    .split(", ")
                    .map((coord) => parseFloat(coord.trim()));

                latlng = L.latLng(lat, lng);

                console.log("Destination: ", latlng);
                setDestination(latlng);
            }
        })
        .catch((error) => {
            console.error("Błąd:", error);
            alert("Failed to fetch data", error);
        });
}

function addBtn() {
    idOfEditedRoute = -1;
    changeView(false);
    console.log("Add btn");
}

async function addRoute(data) {
    fetch("/page/routes/validate-driver-and-vehicle/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    })
        .then((response) => {
            console.log("Response status:", response.status);
            console.log("Response ok:", response.ok);
        })
        .then((responseText) => {
            console.log("Raw response:", responseText);

            window.location.reload();
        })
        .catch((error) => {
            console.error("Error:", error);
            alert("Failed to add route");
        });
}

async function updateRoute(data) {
    data["id"] = idOfEditedRoute;

    console.log(data);

    fetch("/page/routes/validate-driver-and-vehicle/" + idOfEditedRoute, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    })
        .then((response) => {
            console.log("Response status:", response.status);
            console.log("Response ok:", response.ok);
        })
        .then((responseText) => {
            console.log("Raw response:", responseText);

            window.location.reload();
        })
        .catch((error) => {
            console.error("Error:", error);
            alert("Failed to add route");
        });
}

function cancelEdit() {
    changeView(true);
}

function changeView(hideForm) {
    const driverList = document.getElementById("routes-table");
    const formBox = document.getElementById("edit-box");

    if (hideForm) {
        //Hides form, shows list
        driverList.classList.remove("d-none");
        formBox.classList.add("d-none");

        addSelectOption("form-assignedDriver", -1, "None", false, true);
        addSelectOption("form-assignedVehicle", -1, "None", false, true);

        clearMarkers();
    } else {
        //Shows form, hides list
        driverList.classList.add("d-none");
        formBox.classList.remove("d-none");

        initializeMap();
        map.invalidateSize();

        getAvaliableDriversAndVehicles();
    }
}

async function deleteRoute(e) {
    idOfEditedRoute =
        e.target.parentElement.querySelector("td.route-id").textContent;
    fetch("/api/routes/" + idOfEditedRoute, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
    })
        .then((response) => {
            console.log("Status: ", response);

            window.location.reload();
        })
        .catch((error) => {
            console.error("Error:", error);
        });
}

async function setCurrentDriverAndvehicle(driverId, vehicleId) {
    console.log("Getting driver: ", driverId, " and vehicle: ", vehicleId);

    fetch(
        "/page/routes/current-driver-and-vehicle/" + driverId + "/" + vehicleId
    )
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error: " + response.status);
            }
            return response.json();
        })
        .then((data) => {
            if (data.currentDriver["id"] != -1)
                addSelectOption(
                    "form-assignedDriver",
                    data.currentDriver["id"],
                    data.currentDriver["name"] +
                        " " +
                        data.currentDriver["lastName"],
                    true
                );

            if (data.currentVehicle["id"] != -1)
                addSelectOption(
                    "form-assignedVehicle",
                    data.currentVehicle["id"],
                    data.currentVehicle["licensePlate"] +
                        " - " +
                        data.currentVehicle["brand"] +
                        " - " +
                        data.currentVehicle["model"],
                    true
                );
            return data;
        })
        .catch((error) => {
            console.log("Error: ", error);
        });
}

function addSelectOption(
    formElement,
    optionValue,
    optionText,
    isSelected = false,
    clearSelectBeforeAdding = false
) {
    select = document.getElementById(formElement);

    if (clearSelectBeforeAdding) select.length = 0;

    option = document.createElement("option");
    option.value = optionValue;
    option.textContent = optionText;
    option.selected = isSelected;

    select.appendChild(option);
}
