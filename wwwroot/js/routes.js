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
        console.error("Błąd geokodowania:", error);
    }
}

async function calculateRoute() {
    if (!departureCoords || !destinationCoords) {
        alert("Wybierz oba punkty na mapie");
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
        alert("Błąd obliczania trasy: " + error.message);
    }
}

async function getAvaliableDriversAndVehicles() {
    console.log();

    const data = fetch("/page/routes/avaliable-drivers-and-vehicles")
        .then((response) => {
            if (!response.ok) {
                throw new Error("Błąd HTTP: " + response.status);
            }
            return response.json();
        })
        .then((data) => {
            const drivers = data.avaliableDrivers;
            const vehicles = data.avaliableVehicles;

            console.log("Kierowcy:", drivers);
            console.log("Pojazdy:", vehicles);

            data.avaliableDrivers.forEach((element) => {
                const select = document.getElementById("form-assignedDriver");
                const opcja = document.createElement("option");
                opcja.value = element["id"];
                opcja.textContent = element["name"] + " " + element["lastName"];
                select.appendChild(opcja);
            });

            data.avaliableVehicles.forEach((element) => {
                const select = document.getElementById("form-assignedVehicle");
                const opcja = document.createElement("option");
                opcja.value = element["id"];
                opcja.textContent =
                    element["licensePlate"] +
                    " - " +
                    element["brand"] +
                    " - " +
                    element["model"];
                select.appendChild(opcja);
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
    changeView(false);
    idOfEditedRoute =
        //e.target.parentElement.getElementById("route-id").textContent; //querySelector(".route-id").textContent;
        e.target.parentElement.querySelector("td.route-id").textContent;

    // GET/id
    fetch("/api/routes/" + idOfEditedRoute)
        .then((response) => {
            if (!response.ok) {
                throw new Error("Błąd HTTP: " + response.status);
            }
            return response.json(); // Parsuje odpowiedź JSON
        })
        .then((data) => {
            console.log("Dane trasy:", data);

            document.getElementById("form-departure").value =
                data.departure || "";
            document.getElementById("form-destination").value =
                data.destination || "";
            document.getElementById("form-assignedVehicle").value =
                data.assignedVehicle || "";
            document.getElementById("form-assignedDriver").value =
                data.assignedDriver || "";
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
            alert("Nie udało się pobrać danych", error);
        });
}

function addBtn() {
    idOfEditedRoute = -1;
    changeView(false);
    console.log("Add btn");
}

async function addRoute(data) {
    fetch("/api/routes", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data), // Convert to JSON string
    })
        .then((response) => {
            console.log("Response status:", response.status); // Sprawdź status
            console.log("Response ok:", response.ok);
        })
        .then((responseText) => {
            console.log("Raw response:", responseText);

            //window.location.reload();
        })
        .catch((error) => {
            console.error("Błąd:", error);
            alert("Nie udało się dodac trasy");
        });

    return "Adding func";
}

async function updateRoute(data) {
    data["id"] = idOfEditedRoute;

    console.log(data);

    fetch("/api/routes/" + idOfEditedRoute, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data), // Convert to JSON string
    })
        .then((response) => {
            console.log("Response status:", response.status); // Sprawdź status
            console.log("Response ok:", response.ok);
        })
        .then((responseText) => {
            console.log("Raw response:", responseText);

            // window.location.reload();
        })
        .catch((error) => {
            console.error("Błąd:", error);
            alert("Nie udało się dodac trasy");
        });

    return "Updating func";
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

        let select = document.getElementById("form-assignedDriver");
        select.length = 0;
        let opcja = document.createElement("option");
        opcja.value = -1;
        opcja.textContent = "None";
        select.appendChild(opcja);

        select = document.getElementById("form-assignedVehicle");
        select.length = 0;
        opcja = document.createElement("option");
        opcja.value = -1;
        opcja.textContent = "None";
        select.appendChild(opcja);

        clearMarkers();
    } else {
        //Shows form, hides list
        driverList.classList.add("d-none");
        formBox.classList.remove("d-none");

        initializeMap();
        map.invalidateSize();
        /*setTimeout(() => {
            initializeMap();
            map.invalidateSize(); // Odśwież mapę
        }, 100);*/

        const data = getAvaliableDriversAndVehicles();
    }
}

async function deleteRoute(e) {
    idOfEditedRoute =
        //e.target.parentElement.querySelector(".route-id").textContent;
        e.target.parentElement.querySelector("td.route-id").textContent;
    fetch("/api/routes/" + idOfEditedRoute, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
    })
        .then((response) => {
            console.log("Status: ", response);

            //window.location.reload();
        })
        .catch((error) => {
            console.error("Błąd:", error);
        });
}
