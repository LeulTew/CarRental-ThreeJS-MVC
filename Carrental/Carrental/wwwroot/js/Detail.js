const $document = document;
const $chatbot = $document.querySelector(".chatbot");
const $chatbotMessageWindow = $document.querySelector(".chatbot__message-window");
const $chatbotHeader = $document.querySelector(".chatbot__header");
const $chatbotMessages = $document.querySelector(".chatbot__messages");
const $chatbotInput = $document.querySelector(".chatbot__input");
const $chatbotSubmit = $document.querySelector(".chatbot__submit");
const botLoadingDelay = 1000;
const botReplyDelay = 2000;

document.addEventListener("keypress", event => {
    if (event.which == 13) validateMessage();
}, false);

$chatbotHeader.addEventListener("click", () => {
    toggle($chatbot, "chatbot--closed");
    $chatbotInput.focus();
}, false);

$chatbotSubmit.addEventListener("click", () => {
    validateMessage();
}, false);

const toggle = (element, klass) => {
    const classes = element.className.match(/\S+/g) || [];
    index = classes.indexOf(klass);
    index >= 0 ? classes.splice(index, 1) : classes.push(klass);
    element.className = classes.join(" ");
};

const userMessage = content => {
    $chatbotMessages.innerHTML += `<li class='is-user animation'><p class='chatbot__message'>${content}</p><span class='chatbot__arrow chatbot__arrow--right'></span></li>`;
};

const aiMessage = (content, isLoading = false, delay = 0) => {
    setTimeout(() => {
        removeLoader();
        $chatbotMessages.innerHTML += `<li class='is-ai animation' id='${isLoading ? "is-loading" : ""}'><div class="is-ai__profile-picture"><svg class="icon-avatar" viewBox="0 0 32 32"><use xlink:href="#avatar" /></svg></div><span class='chatbot__arrow chatbot__arrow--left'></span><div class='chatbot__message'>${content}</div></li>`;
        scrollDown();
    }, delay);
};

const removeLoader = () => {
    let loadingElem = document.getElementById("is-loading");
    if (loadingElem) $chatbotMessages.removeChild(loadingElem);
};

const escapeScript = unsafe => {
    const safeString = unsafe.replace(/</g, " ").replace(/>/g, " ").replace(/&/g, " ").replace(/"/g, " ").replace(/\\/, " ").replace(/\s+/g, " ");
    return safeString.trim();
};

const validateMessage = () => {
    const text = $chatbotInput.value;
    const safeText = text ? escapeScript(text) : "";
    if (safeText.length && safeText !== " ") {
        resetInputField();
        userMessage(safeText);
        processResponse(safeText); // Directly process the response
    }
    scrollDown();
    return;
};

const processResponse = text => {
    let output = "";
    const speech = text.toLowerCase();
    if (speech.includes("time") || speech.includes("hour")) {
        output += "<p>You can come to our office or rental on the day booked, and you will return the car on the last day.</p>";
    } else if (speech.includes("help") || speech.includes("how")) {
        output += "<p>For assistance with renting or any inquiries, please contact us at 0966235333.</p>";
    } else if (speech.includes("location") || speech.includes("address")) {
        output += "<p>Our office is located at Megenagia, you can also find us on Google Maps.</p>";
    } else if (speech.includes("payment") || speech.includes("price")) {
        output += "<p>We accept various payment methods including cash, credit card, and mobile payment. Prices may vary depending on the type of car and rental duration.</p>";
    } else if (speech.includes("insurance")) {
        output += "<p>All our rentals include basic insurance coverage. Additional insurance options are available for purchase.</p>";
    } else if (speech.includes("reservation") || speech.includes("booking")) {
        output += "<p>To make a reservation, you can book online through our website or contact our customer service team.</p>";
    } else if (speech.includes("hello") || speech.includes("hey") || speech.includes("hi")) {
        output += "<p>Hello! How can I assist you today?</p>";
    } else if (speech.includes("how are you")) {
        output += "<p>I'm doing well, thank you for asking! How can I help you?</p>";
    }

    // Handle expressions of gratitude
    else if (speech.includes("thank you") || speech.includes("thanks")) {
        output += "<p>You're welcome! If you need any further assistance, feel free to ask.</p>";
    }

    // Provide information about the available car models
    else if (speech.includes("car models") || speech.includes("car types") || speech.includes("available cars")) {
        output += "<p>We offer a variety of car models including sedans, SUVs, hatchbacks, and luxury vehicles. Is there a specific model or type you're interested in?</p>";
    } else if (speech.includes("sedan")) {
        output += "<p>We have a selection of sedan models available for rent, including popular brands such as Toyota Corolla and Honda Civic.</p>";
    } else if (speech.includes("suv")) {
        output += "<p>We offer SUV rentals suitable for both urban and off-road adventures. Our SUV fleet includes models like Toyota RAV4 and Ford Explorer.</p>";
    } else if (speech.includes("hatchback")) {
        output += "<p>Our hatchback rentals provide versatility and fuel efficiency, perfect for city driving. Explore options like Volkswagen Golf and Hyundai i30.</p>";
    }

    // Address inquiries about additional fees
    else if (speech.includes("fees") || speech.includes("extra charges")) {
        output += "<p>Additional fees may apply for services such as GPS navigation, car insurance upgrades, and additional driver fees. Would you like more details?</p>";
    }

    // Assist with questions regarding fuel policy
    else if (speech.includes("fuel") || speech.includes("gas")) {
        output += "<p>Our fuel policy typically requires renters to return the car with the same level of fuel as when it was rented. However, we also offer options for prepaid fuel. Which option would you like to know more about?</p>";
    }

    // Provide information about age requirements for renting
    else if (speech.includes("age") || speech.includes("age requirement") || speech.includes("old")) {
        output += "<p>The minimum age to rent a car is typically 21 years old, although it may vary depending on the rental location and car category. Do you need more details about our age requirements?</p>";
    }

    // Assist with inquiries about special offers or discounts
    else if (speech.includes("offers") || speech.includes("discounts")) {
        output += "<p>We often have special offers and discounts available for our customers. Please visit our website or contact our customer service team to learn more about our current promotions.</p>";
    }

    // Handle requests for roadside assistance
    else if (speech.includes("roadside assistance") || speech.includes("emergency")) {
        output += "<p>We provide 24/7 roadside assistance for our customers in case of emergencies or breakdowns. Do you need immediate assistance?</p>";
    }

    else if (speech.includes("What cars you got?") || speech.includes("car options") || speech.includes("available cars") || speech.includes("car selection")) {
        output += "<p>We offer a variety of car models including sedans, SUVs, hatchbacks, and luxury vehicles. Is there a specific model or type you're interested in?</p>";
    }

    else if (speech.includes("Where can I pick up a car?") || speech.includes("pickup location") || speech.includes("pick-up point") || speech.includes("collection point")) {
        output += "<p>We have multiple pick-up locations conveniently located throughout the city. You can choose the location that is most convenient for you during the booking process. Would you like assistance finding a location?</p>";
    }

    else if (speech.includes("Can I pay with my card?") || speech.includes("payment options") || speech.includes("pay with card") || speech.includes("credit card")) {
        output += "<p>We accept various payment methods including cash, credit card, and mobile payment. You can choose your preferred payment option during the booking process.</p>";
    }

    else if (speech.includes("What if the car breaks down?") || speech.includes("car breakdown") || speech.includes("car trouble") || speech.includes("vehicle problem")) {
        output += "<p>If the car breaks down during your rental period, please contact our roadside assistance hotline immediately. We provide 24/7 support for emergencies and breakdowns.</p>";
    }


    // Address questions about rental duration and flexibility
    else if (speech.includes("rental duration") || speech.includes("how long can I rent")) {
        output += "<p>Rental durations vary depending on your needs, ranging from daily rentals to long-term leases. We offer flexible rental options to accommodate your schedule. Would you like more information about our rental durations?</p>";
    }

    // Assist with inquiries about vehicle maintenance
    else if (speech.includes("maintenance") || speech.includes("car condition")) {
        output += "<p>We regularly maintain our vehicles to ensure they are in optimal condition for our customers. If you have specific concerns about a vehicle's condition, please let us know so we can address them accordingly.</p>";
    }

    // Handle questions about pick-up and drop-off locations
    else if (speech.includes("pick-up") || speech.includes("drop-off") || speech.includes("location")) {
        output += "<p>We have multiple pick-up and drop-off locations conveniently located throughout the city. You can choose the location that is most convenient for you during the booking process. Would you like assistance finding a location?</p>";
    }

    // Provide information about seasonal promotions or offers
    else if (speech.includes("seasonal offers") || speech.includes("current promotions")) {
        output += "<p>During certain seasons or holidays, we may offer special promotions or discounts on car rentals. Keep an eye on our website or subscribe to our newsletter to stay updated on our latest offers.</p>";
    }

    // Handle inquiries about rental duration and flexibility
    else if (speech.includes("how long can I rent")) {
        output += "<p>Rental durations vary depending on your needs, ranging from daily rentals to long-term leases. We offer flexible rental options to accommodate your schedule. Would you like more information about our rental durations?</p>";
    }

    // Address questions about vehicle maintenance
    else if (speech.includes("car maintenance") || speech.includes("vehicle condition")) {
        output += "<p>We regularly maintain our vehicles to ensure they are in optimal condition for our customers. If you have specific concerns about a vehicle's condition, please let us know so we can address them accordingly.</p>";
    }

    // Handle questions about pick-up and drop-off locations
    else if (speech.includes("pick-up") || speech.includes("drop-off") || speech.includes("location")) {
        output += "<p>We have multiple pick-up and drop-off locations conveniently located throughout the city. You can choose the location that is most convenient for you during the booking process. Would you like assistance finding a location?</p>";
    }

    // Provide information about seasonal promotions or offers
    else if (speech.includes("seasonal offers") || speech.includes("current promotions")) {
        output += "<p>During certain seasons or holidays, we may offer special promotions or discounts on car rentals. Keep an eye on our website or subscribe to our newsletter to stay updated on our latest offers.</p>";
    }

    // Handle inquiries about child safety seats or other equipment
    else if (speech.includes("child seat") || speech.includes("equipment rental")) {
        output += "<p>We offer child safety seats and other equipment rentals to ensure the safety and comfort of your family during your travels. Let us know your requirements, and we'll be happy to assist you.</p>";
    }

    // Assist with questions about driving requirements or licenses
    else if (speech.includes("driver's license") || speech.includes("driving requirements")) {
        output += "<p>To rent a car, you typically need a valid driver's license and a major credit card. International renters may need an international driving permit. Do you need more information about our driving requirements?</p>";
    }

    // Address inquiries about luxury or premium car rentals
    else if (speech.includes("luxury cars") || speech.includes("luxury car") || speech.includes("premium rentals")) {
        output += "<p>We offer a selection of luxury and premium cars for those looking for an elevated driving experience. Whether you're attending a special event or want to indulge in luxury, we have the perfect car for you.</p>";
    }

    // Handle questions about accessibility features for disabled customers
    else if (speech.includes("accessibility") || speech.includes("disabled customers")) {
        output += "<p>We strive to accommodate all customers, including those with disabilities. Please let us know your specific needs, and we'll do our best to provide the necessary assistance and accommodations.</p>";
    }

    // Provide information about company policies or terms and conditions
    else if (speech.includes("policies") || speech.includes("terms and conditions")) {
        output += "<p>Our company policies and terms and conditions are designed to ensure a smooth and fair rental experience for all customers. You can find detailed information about our policies on our website or contact our customer service team for assistance.</p>";
    }


    // Assist with questions about vehicle availability or reservations
    else if (speech.includes("availability") || speech.includes("reservation status")) {
        output += "<p>We recommend making a reservation in advance to ensure the availability of your desired vehicle. You can check the availability and status of your reservation online or contact our customer service team for assistance.</p>";
    }

    // Handle inquiries about corporate or business accounts
    else if (speech.includes("corporate accounts") || speech.includes("business rentals")) {
        output += "<p>We offer corporate and business rental accounts with special rates and benefits for companies and organizations. If you're interested in setting up a corporate account, please contact our corporate sales team for more information.</p>";
    }

    // Address questions about international or cross-border rentals
    else if (speech.includes("international rentals") || speech.includes("cross-border")) {
        output += "<p>We offer international and cross-border rentals for customers who plan to travel between different countries or regions. Additional documentation and fees may apply, so please contact our customer service team for assistance with international rentals.</p>";
    }

    // Provide information about insurance coverage options
    else if (speech.includes("insurance coverage") || speech.includes("rental insurance")) {
        output += "<p>We offer various insurance coverage options to provide peace of mind during your rental period. From basic insurance to comprehensive coverage, we have options to suit your needs. Would you like more details about our insurance offerings?</p>";
    } // Handle inquiries about specific car makes
else if (speech.includes("toyota")) {
        output += "<p>We have various Toyota models available for rent, including popular options like the Toyota Corolla, Camry, and RAV4.</p>";
    } else if (speech.includes("honda")) {
        output += "<p>Our Honda lineup includes models such as the Honda Civic, Accord, and CR-V, known for their reliability and performance.</p>";
    } else if (speech.includes("ford")) {
        output += "<p>Explore our selection of Ford vehicles, including the Ford Mustang, Explorer, and Fusion, offering versatility and style.</p>";
    }

    // Address questions about rental rates and pricing
    else if (speech.includes("rental rates") || speech.includes("pricing")) {
        output += "<p>Rental rates vary depending on factors such as the car model, rental duration, and additional services requested. You can find detailed pricing information on our website or contact our customer service team for assistance.</p>";
    }

    // Assist with inquiries about mileage limits
    else if (speech.includes("mileage limit") || speech.includes("km limit")) {
        output += "<p>Our rental agreements typically include a daily mileage limit, with options to purchase additional mileage if needed. Would you like to know more about our mileage policies?</p>";
    }

    // Provide information about one-way rentals
    else if (speech.includes("one-way rental") || speech.includes("drop-off location")) {
        output += "<p>We offer one-way rentals, allowing you to pick up a car at one location and drop it off at another. Additional fees may apply based on the drop-off location. Please contact our customer service team for assistance with one-way rentals.</p>";
    }

    // Handle inquiries about vehicle upgrades or downgrades
    else if (speech.includes("upgrade") || speech.includes("downgrade")) {
        output += "<p>We offer vehicle upgrades and downgrades based on availability and your preferences. Whether you need a larger vehicle for a family trip or a smaller car for city driving, we can accommodate your request.</p>";
    }

    // Assist with questions about long-term rentals
    else if (speech.includes("long-term rental") || speech.includes("monthly rental")) {
        output += "<p>We provide long-term rental options for customers needing a car for an extended period. Enjoy discounted rates and flexible terms with our monthly rental plans.</p>";
    }

    // Address inquiries about additional driver options
    else if (speech.includes("additional driver")) {
        output += "<p>You can add additional drivers to your rental agreement for a small fee. All drivers must meet our age and licensing requirements. Would you like to know more about adding an additional driver?</p>";
    }

    // Provide information about car rental rewards programs
    else if (speech.includes("rewards program") || speech.includes("loyalty program")) {
        output += "<p>Join our car rental rewards program to earn points on every rental, redeemable for discounts, free upgrades, and other exclusive benefits. Enroll in our loyalty program today!</p>";
    }

    // Handle inquiries about exotic or specialty car rentals
    else if (speech.includes("exotic cars") || speech.includes("specialty rentals")) {
        output += "<p>Experience luxury and excitement with our selection of exotic and specialty car rentals. From sports cars to luxury sedans, we have unique vehicles to elevate your driving experience.</p>";
    }

    // Handle questions about fuel-efficient vehicles
    else if (speech.includes("fuel-efficient cars") || speech.includes("fuel-efficient car") || speech.includes("hybrid rentals")) {
        output += "<p>Opt for one of our fuel-efficient or hybrid rental cars to save on fuel costs and reduce your environmental footprint. Explore our eco-friendly options for your next rental.</p>";
    }

    // Provide information about off-road or adventure rentals
    else if (speech.includes("off-road vehicles") || speech.includes("adventure rentals")) {
        output += "<p>Embark on your next adventure with our off-road and adventure rental vehicles. Choose from rugged SUVs and 4x4 trucks designed to handle challenging terrain.</p>";
    }

    // Handle inquiries about renting accessories or equipment
    else if (speech.includes("rent accessories") || speech.includes("equipment rental")) {
        output += "<p>In addition to cars, we offer accessories and equipment rentals such as roof racks, bike racks, and camping gear to enhance your travel experience. Let us know your requirements, and we'll have everything ready for you.</p>";
    }

    // Handle questions about pet-friendly rentals
    else if (speech.includes("pet-friendly cars") || speech.includes("travel with pets")) {
        output += "<p>We understand that pets are part of the family, which is why we offer pet-friendly rental options. Travel with your furry friend comfortably in our designated pet-friendly vehicles.</p>";
    }

    // Handle inquiries about electric or hybrid car rentals
    else if (speech.includes("electric cars") || speech.includes("hybrid rentals")) {
        output += "<p>Go green with our electric and hybrid car rental options. Enjoy the benefits of eco-friendly driving while exploring the city or embarking on a road trip.</p>";
    }

    // Provide information about in-car entertainment options
    else if (speech.includes("entertainment system") || speech.includes("car amenities")) {
        output += "<p>Our rental cars come equipped with modern entertainment systems, including Bluetooth connectivity, USB ports, and touchscreen displays, to keep you entertained during your journey.</p>";
    }

    // Handle inquiries about sports car rentals
    else if (speech.includes("sports cars") || speech.includes("convertible rentals")) {
        output += "<p>Feel the thrill of the open road with our sports car and convertible rentals. Experience the excitement of driving a high-performance vehicle on your next adventure.</p>";
    }

    // Handle questions about pickup truck rentals
    else if (speech.includes("pickup trucks") || speech.includes("truck rentals")) {
        output += "<p>Need to haul cargo or tackle tough jobs? Rent one of our pickup trucks for reliable performance and versatility. Whether you're moving furniture or transporting equipment, our trucks have you covered.</p>";
    }

    // Handle inquiries about luxury SUV rentals
    else if (speech.includes("luxury suvs") || speech.includes("premium suv rentals")) {
        output += "<p>Experience luxury and comfort on your travels with our selection of premium SUV rentals. From spacious interiors to advanced features, our luxury SUVs offer the ultimate driving experience.</p>";
    }

    // Handle questions about vintage or classic car rentals
    else if (speech.includes("vintage cars") || speech.includes("classic car rentals")) {
        output += "<p>Travel back in time with our vintage and classic car rental options. Cruise the streets in style with iconic vehicles from different eras, perfect for special occasions or nostalgic road trips.</p>";
    }

    // Provide information about car rental policies for young drivers
    else if (speech.includes("young drivers") || speech.includes("under 25")) {
        output += "<p>Drivers under the age of 25 may be subject to additional fees or restrictions when renting a car. Please contact our customer service team for information about our policies for young drivers.</p>";
    }
// Handle inquiries about customer service hours
else if (speech.includes("customer service hours") || speech.includes("support hours") || speech.includes("service hours")) {
        output += "<p>Our customer service team is available to assist you during our regular business hours, which are Monday through Friday, 9:00 AM to 6:00 PM local time. Feel free to reach out to us during these hours for any assistance you may need.</p>";
    }

    // Assist with questions about lost and found items
    else if (speech.includes("lost and found") || speech.includes("lost item") || speech.includes("found item")) {
        output += "<p>If you've lost or found an item in one of our rental vehicles, please contact our customer service team as soon as possible. We'll do our best to assist you in retrieving lost items or returning found items to their rightful owners.</p>";
    }

    // Provide information about customer feedback and surveys
    else if (speech.includes("customer feedback") || speech.includes("feedback survey") || speech.includes("customer survey")) {
        output += "<p>We value your feedback and strive to continuously improve our services. You may receive a feedback survey after your rental experience to share your thoughts and suggestions. Your input helps us enhance the quality of our services.</p>";
    }

    // Handle inquiries about vehicle recalls or safety notices
    else if (speech.includes("vehicle recall") || speech.includes("safety notice") || speech.includes("recall information")) {
        output += "<p>If you have concerns about vehicle recalls or safety notices, please contact our customer service team for assistance. We'll provide you with the necessary information and guidance to ensure your safety and satisfaction.</p>";
    }

    // Assist with questions about account management or login issues
    else if (speech.includes("account management") || speech.includes("login issues") || speech.includes("password reset")) {
        output += "<p>If you're experiencing issues with your account or need assistance with account management, please contact our customer service team. We're here to help you resolve any login issues or update your account information as needed.</p>";
    }

    // Provide information about customer support channels
    else if (speech.includes("contact customer support") || speech.includes("support channels") || speech.includes("get in touch with support")) {
        output += "<p>You can contact our customer support team via phone, email, or live chat for assistance with any questions or concerns you may have. Our dedicated support staff is here to help you whenever you need assistance.</p>";
    }

    // Handle inquiries about billing or payment disputes
    else if (speech.includes("billing inquiry") || speech.includes("payment dispute") || speech.includes("billing error")) {
        output += "<p>If you have questions or concerns about billing or payment disputes, please contact our billing department for assistance. We'll review your account and address any discrepancies or issues promptly.</p>";
    }

    // Assist with questions about loyalty program membership
    else if (speech.includes("loyalty program enrollment") || speech.includes("rewards program registration") || speech.includes("join rewards program")) {
        output += "<p>To enroll in our loyalty program and start earning rewards, please visit our website or contact our customer service team for assistance. We'll help you register for the program and answer any questions you may have.</p>";
    }

    // Provide information about corporate or group bookings
    else if (speech.includes("group reservations") || speech.includes("corporate bookings") || speech.includes("group discounts")) {
        output += "<p>If you're planning a group event or corporate gathering and need to book multiple vehicles, please contact our group reservations department for special rates and assistance with coordinating your bookings.</p>";
    }

    // Handle inquiries about customer service policies or procedures
    else if (speech.includes("customer service policy") || speech.includes("service procedure") || speech.includes("support guidelines")) {
        output += "<p>Our customer service policies and procedures are designed to ensure prompt and effective assistance for all our customers. If you have any questions about our service guidelines, please don't hesitate to contact us for clarification.</p>";
    }
    // Handle inquiries about vehicle availability
    else if (speech.includes("available cars") || speech.includes("car availability") || speech.includes("cars to rent")) {
        output += "<p>We have a wide range of vehicles available for rent. What type of car are you looking for?</p>";
    }

    // Assist with questions about rental rates
    else if (speech.includes("rental rates") || speech.includes("pricing")) {
        output += "<p>Rental rates vary depending on the type of vehicle and rental duration. Can you provide more details?</p>";
    }

    // Address inquiries about pick-up and drop-off locations
    else if (speech.includes("pick up") || speech.includes("drop off") || speech.includes("nearest office")) {
        output += "<p>We have multiple pick-up and drop-off locations for your convenience. Where would you like to pick up or drop off?</p>";
    }

    // Provide information about payment methods
    else if (speech.includes("payment") || speech.includes("pay")) {
        output += "<p>We accept various payment methods including credit card, debit card, and cash. Which payment method would you prefer?</p>";
    }

    // Handle questions about rental duration
    else if (speech.includes("rental period") || speech.includes("duration")) {
        output += "<p>You can rent a car for as short as a day or as long as several weeks or months. How long do you need the car for?</p>";
    }

    // Assist with inquiries about additional fees
    else if (speech.includes("fees") || speech.includes("charges") || speech.includes("costs")) {
        output += "<p>There may be additional fees for services like insurance upgrades or additional equipment rentals. Would you like more information?</p>";
    }

    // Address questions about insurance coverage
    else if (speech.includes("insurance") || speech.includes("coverage")) {
        output += "<p>All rentals come with basic insurance coverage, but you can choose to upgrade for additional protection. Do you have any specific concerns about insurance?</p>";
    }

    // Provide information about age requirements
    else if (speech.includes("age") || speech.includes("old")) {
        output += "<p>Generally, you need to be at least 21 years old to rent a car. Are you over 21?</p>";
    }

    // Handle inquiries about roadside assistance
    else if (speech.includes("emergency") || speech.includes("breakdown")) {
        output += "<p>We provide 24/7 roadside assistance in case of emergencies or breakdowns. Do you need immediate assistance?</p>";
    }


    // Handle general questions or inquiries not covered by specific topics
    else {
        output += "<p>I'm sorry, I didn't quite catch that. Could you please rephrase your question or let me know how I can assist you with customer service-related inquiries?</p>";
    }

    aiMessage(output);
};


const resetInputField = () => {
    $chatbotInput.value = "";
};

const scrollDown = () => {
    const distanceToScroll = $chatbotMessageWindow.scrollHeight - ($chatbotMessages.lastChild.offsetHeight + 60);
    $chatbotMessageWindow.scrollTop = distanceToScroll;
    return false;
};
