let idOfEditedVehicle = -1;

document
    .getElementById("vehicle-form")
    .addEventListener("submit", async function (e) {
        e.preventDefault();

        try {
            const formData = new FormData(this);
            const jsonData = Object.fromEntries(formData);

            const result =
                idOfEditedVehicle != -1
                    ? await updateVehicle(jsonData)
                    : await addVehicle(jsonData);
            console.log("Submit vehicle triggered: ", jsonData);

            this.reset();
            changeView(true);
            
        } catch (error) {
            console.error("Error:", error);
        }
    });

document.addEventListener("DOMContentLoaded", function () {
    document
        .getElementById("vehicles-table")
        .addEventListener("click", function (e) {
            if (e.target.classList.contains("delete-btn")) {
                deleteVehicle(e);
            }
            if (e.target.classList.contains("edit-btn")) {
                editVehicle(e);
            }
        });
});

function editVehicle(e) {
    changeView(false);

    idOfEditedVehicle =
        e.target.parentElement.querySelector("td.vehicle-id").textContent;

    fetch("/api/vehicles/" + idOfEditedVehicle)
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error: " + response.status);
            }
            return response.json();
        })
        .then((data) => {
            console.log("Vehicle data:", data);

            document.getElementById("form-brand").value = data.brand || "";
            document.getElementById("form-model").value = data.model || "";
            document.getElementById("form-year").value = data.year || "";
            document.getElementById("form-mileage").value = data.mileage || "";
            document.getElementById("form-licensePlate").value =
                data.licensePlate || "";
            document.getElementById("form-avgSpeed").value =
                data.averageSpeed || "";
        })
        .catch((error) => {
            console.error("Error:", error);
            alert("Failed to fetch data", error);
        });
}

function addBtn() {
    idOfEditedVehicle = -1;
    changeView(false);
    console.log("Add btn");
}

function addVehicle(data) {
    fetch("/api/vehicles", {
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
            console.error("Błąd:", error);
            alert("Failed to add vehicle");
        });
}

function updateVehicle(data) {
    data["Id"] = idOfEditedVehicle;

    fetch("/api/vehicles/" + idOfEditedVehicle, {
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

            window.location.reload();
        })
        .catch((error) => {
            console.error("Error:", error);
            alert("Failed to add vehicle");
        });
}

function cancelEdit() {
    changeView(true);
}

function changeView(hideForm) {
    const driverList = document.getElementById("vehicles-table");
    const formBox = document.getElementById("edit-box");

    if (hideForm) {
        //Hides form, shows list
        driverList.classList.remove("d-none");
        formBox.classList.add("d-none");
    } else {
        //Shows form, hides list
        driverList.classList.add("d-none");
        formBox.classList.remove("d-none");
    }
}

function deleteVehicle(e) {
    idOfEditedVehicle =
        e.target.parentElement.querySelector("td.vehicle-id").textContent;

    fetch("/api/vehicles/" + idOfEditedVehicle, {
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
