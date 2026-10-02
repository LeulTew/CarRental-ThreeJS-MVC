
document.getElementById("reset-btn").addEventListener("click", function () {
    window.location.reload(); // Reload the page
});


//carMake

document.addEventListener("DOMContentLoaded", function () {
    const radioSelected = rentTab.checked ? false : true;

    // Event listener for reset button
    const resetBtn = document.getElementById("reset-btn");
    resetBtn.addEventListener("click", function () {
        // Reset car make and car model selects to "All"
        carMakeSelect.value = "All";
        carModelSelect.value = "All";
        filterCards();
        filterCars(radioSelected);
    });

    // Initial filter based on selected tab
    filterCars(radioSelected);
});

const carMakeSelect = document.getElementById("car-make");
const carModelSelect = document.getElementById("car-model");

// Function to submit the form
function submitForm() {
    document.getElementById("filter-form").submit();
}

// Event listener for car make select
carMakeSelect.addEventListener("change", function () {
    submitForm(); // Submit the form when car make is changed
});

// Event listener for car model select
carModelSelect.addEventListener("change", function () {
    submitForm(); // Submit the form when car model is changed
});




// Function to toggle the display of the filter section
document.addEventListener("DOMContentLoaded", function () {
    const filterBtn = document.querySelector(".filter-b");
    const filterSection = document.querySelector(".filter");

    filterBtn.addEventListener("click", function () {
        filterSection.style.display = filterSection.style.display === "none" ? "block" : "none";
    });
});



//Car Make&Model
// Function to update car models based on the selected make
function updateCarModels(select) {
    var make = select.value;
    var carModelSelect = document.getElementById("car-model");

    // Clear previous options
    while (carModelSelect.firstChild) {
        carModelSelect.removeChild(carModelSelect.firstChild);
    }

    // Add 'All' option
    var allOption = document.createElement("option");
    allOption.text = "All";
    allOption.value = "All";
    carModelSelect.appendChild(allOption);

    // Fetch models based on the selected make
    if (make !== "All") {
        fetch(`/api/CarModels?make=${make}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(model => {
                    var option = document.createElement("option");
                    option.text = model;
                    option.value = model;
                    carModelSelect.appendChild(option);
                });
            });
    }
}




//Filter By Price
document.addEventListener("DOMContentLoaded", function () {
    let inputLeft = document.getElementById("price-range-min");
    let inputRight = document.getElementById("price-range-max");
    let range = document.querySelector(".slider > .range");
    let priceFrom = document.querySelector(".price-from");
    let priceTo = document.querySelector(".price-to");

    function setLeftValue() {
        let _this = inputLeft,
            min = parseInt(_this.min),
            max = parseInt(_this.max);

        _this.value = Math.min(
            parseInt(_this.value),
            parseInt(inputRight.value) - 50
        );
        priceFrom.textContent = `Br.${_this.value}`;

        let percent = ((_this.value - min) / (max - min)) * 100;

        range.style.left = percent + "%";
    }

    setLeftValue();

    function setRightValue() {
        let _this = inputRight,
            min = parseInt(_this.min),
            max = parseInt(_this.max);

        _this.value = Math.max(parseInt(_this.value), parseInt(inputLeft.value) + 50);
        priceTo.textContent = `Br.${_this.value}`;

        let percent = ((_this.value - min) / (max - min)) * 100;

        range.style.right = 100 - percent + "%";
    }

    setRightValue();

    inputLeft.addEventListener("input", setLeftValue);
    inputRight.addEventListener("input", setRightValue);

    inputLeft.addEventListener("mouseover", (e) => {
        inputLeft.classList.add("hover");
    });
    inputLeft.addEventListener("mouseout", (e) => {
        inputLeft.classList.remove("hover");
    });
    inputLeft.addEventListener("mousedown", (e) => {
        inputLeft.classList.add("active");
    });
    inputLeft.addEventListener("mouseup", (e) => {
        inputLeft.classList.remove("active");
    });
    inputLeft.addEventListener("touchstart", (e) => {
        inputLeft.classList.add("active");
    });
    inputLeft.addEventListener("touchend", (e) => {
        inputLeft.classList.remove("active");
    });

    inputRight.addEventListener("mouseover", (e) => {
        inputRight.classList.add("hover");
    });
    inputRight.addEventListener("mouseout", (e) => {
        inputRight.classList.remove("hover");
    });
    inputRight.addEventListener("mousedown", (e) => {
        inputRight.classList.add("active");
    });
    inputRight.addEventListener("mouseup", (e) => {
        inputRight.classList.remove("active");
    });
    inputRight.addEventListener("touchstart", (e) => {
        inputRight.classList.add("active");
    });
    inputRight.addEventListener("touchend", (e) => {
        inputRight.classList.remove("active");
    });
    // Function to submit the form after a delay
    let timer;
    function submitFormWithDelay() {
        clearTimeout(timer);
        timer = setTimeout(() => {
            document.getElementById("filter-form").submit(); // Submit the form after 2 seconds
        }, 2000);
    }

    // Event listeners for price range inputs
    inputLeft.addEventListener("input", submitFormWithDelay);
    inputRight.addEventListener("input", submitFormWithDelay);
});



//Range
let timeout = null;

function delaySubmitForm() {
    clearTimeout(timeout);
    timeout = setTimeout(submitForm, 2000); // Delay submission for 2 seconds (2000 milliseconds)
}

function submitForm() {
    document.getElementById("filter-form").submit();
}


//CarType
function sortCars() {
    // Set the selected option based on the sortBy value
    document.getElementById("filter-form").submit();

}
function filterByType() {
    // Submit the form
    document.getElementById("filter-form").submit();
}


//Color
function filterByColor() {
    // Submit the form
    document.getElementById("filter-form").submit();
}


//isAvail
function filterAvailableNow(checkbox) {
    checkbox.value = checkbox.checked ? "true" : "false";
    document.getElementById("filter-form").submit();
}

function filterByFavorites(checkbox) {
    checkbox.value = checkbox.checked ? "true" : "false";
    document.getElementById("filter-form").submit();
}

//Fav
// JavaScript to handle Rent filter change event
function filterRent(radio) {
    if (radio.checked) {
        document.getElementById("filter-form").submit();
        document.getElementById("filter-form2").submit();
    }
}

// JavaScript to handle Buy filter change event
function filterBuy(radio) {
    if (radio.checked) {
        document.getElementById("filter-form").submit();
        document.getElementById("filter-form2").submit();
    }
}




