let idOfEditedDriver = -1;

document.getElementById("driver-form").addEventListener("submit", async function(e) {
    e.preventDefault();

    try {
        const formData = new FormData(this);
        const jsonData = Object.fromEntries(formData);

        const result = (idOfEditedDriver != -1)? (await updateDriver(jsonData)) : (await addDriver(jsonData));
        console.log('Submit driver triggered: ',jsonData);
        this.reset();
        changeView(true);

    } catch (error) {
        console.error('Error:', error);
    }
});

function editDriver(e) {
    //isEditMode = true;
    changeView(false);

    idOfEditedDriver = e.target.parentElement.querySelector(".driver-id").textContent;

    // GET/id
    fetch("api/drivers/" + idOfEditedDriver).then(response => {
            if (!response.ok) {
                throw new Error('Błąd HTTP: ' + response.status);
            }
            return response.json(); // Parsuje odpowiedź JSON
        }).then(data => {
            console.log('Dane kierowcy:', data);

            document.getElementById('form-firstName').value = data.firstName || '';
            document.getElementById('form-lastName').value = data.lastName || '';
            document.getElementById('form-licenseNumber').value = data.licenseNumber || '';
            document.getElementById('form-licenseExpiryDate').value = data.licenseExpiryDate || '';
            document.getElementById('form-phone').value = data.phoneNumber || '';
            document.getElementById('form-email').value = data.email || '';
      })
        .catch(error => {
            console.error('Błąd:', error);
            alert('Nie udało się pobrać danych',error);
    });
}

function addBtn() {
    idOfEditedDriver = -1;
    changeView(false);
    console.log("Add btn");
}

function addDriver(data) {
    fetch("api/drivers", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data) // Convert to JSON string
        })
        .then(response => {
            console.log('Response status:', response.status); // Sprawdź status
            console.log('Response ok:', response.ok);
        })
        .then(responseText => {
            console.log('Raw response:', responseText);
            
            window.location.reload();
        })
        .catch(error => {
            console.error('Błąd:', error);
            alert('Nie udało się dodac kierowcy');
        });

    return "Adding func";
}

function updateDriver(data) {
    data["Id"] = idOfEditedDriver;

    fetch("api/drivers/" + idOfEditedDriver, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data) // Convert to JSON string
        })
        .then(response => {
            console.log('Response status:', response.status); // Sprawdź status
            console.log('Response ok:', response.ok);
        })
        .then(responseText => {
            console.log('Raw response:', responseText);

            window.location.reload();
        })
        .catch(error => {
            console.error('Błąd:', error);
            alert('Nie udało się dodac kierowcy');
        });

    return "Updating func";
}

function cancelEdit() {
    changeView(true);
}

function changeView(hideForm) {

    const driverList = document.getElementById("drivers-table");
    const formBox = document.getElementById("edit-box");
    
    if(hideForm) { //Hides form, shows list
        driverList.classList.remove("d-none");
        formBox.classList.add("d-none");
    } else { //Shows form, hides list
        driverList.classList.add("d-none");
        formBox.classList.remove("d-none");
    }
}

function deleteDriver(e) {
    idOfEditedDriver = e.target.parentElement.querySelector(".driver-id").textContent;

    fetch("api/drivers/" + idOfEditedDriver, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        }
    })
    .then(response => {
        console.log('Status: ', response);

        window.location.reload();
    })
    .catch(error => {
        console.error('Błąd:', error);
    });
}

function phoneMask() {
    const input = this;
    let num = input.value.replace(/\D/g, '');
    input.value = num.substring(0, 3) 
        + (num.length > 3 ? '-' : '') 
        + num.substring(3, 6) 
        + (num.length > 6 ? '-' : '') 
        + num.substring(6, 9);
}

document.querySelectorAll('[type="tel"]').forEach(input => {
    input.addEventListener('keyup', phoneMask);
});