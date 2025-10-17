let currentDotNetHelper = null;
let globeInstance = null;
let globeData = null;
let globeContainer = null;
const globes = {};
let avgLat = 0;
let avgLng = 0;
const offcanvasListeners = new Set();
const scriptLoadStates = new Map();
let clickHandler = null;
let fitResizeHandler = null;

//declared these globally to remove listeners
let autocomplete = null;
let wrapper = null;
let handleWheel = null;
let handleClick = null;
let autocompleteHandler = null;

export function loadScript(src) {
    if (scriptLoadStates.has(src)) {
        return scriptLoadStates.get(src); // return the existing promise
    }

    const existingScript = document.querySelector(`script[src="${src}"]`);
    if (existingScript) {
        const promise = new Promise((resolve) => {
            if (existingScript.hasAttribute("data-loaded")) {
                resolve();
            } else {
                existingScript.addEventListener("load", resolve);
            }
        });
        scriptLoadStates.set(src, promise);
        return promise;
    }

    const script = document.createElement("script");
    script.src = src;
    script.async = true;
    script.defer = true;

    const promise = new Promise((resolve, reject) => {
        script.onload = () => {
            script.setAttribute("data-loaded", "true");
            resolve();
        };
        script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
    });

    document.head.appendChild(script);
    scriptLoadStates.set(src, promise);
    return promise;
}

export function createIntroGlobe(elementId) {
    const container = document.getElementById(elementId);
    if (!container) return;

    const globe = Globe()(container)
        .globeImageUrl('//cdn.jsdelivr.net/npm/three-globe/example/img/earth-blue-marble.jpg')
        .backgroundImageUrl('//unpkg.com/three-globe/example/img/night-sky.png')
        .backgroundColor('rgba(0,0,0,1)');

    const topOffset = 56;

    function getSidebarWidth() {
        const sidebar = document.getElementById('sidebar');
        return sidebar && getComputedStyle(sidebar).display !== 'none'
            ? sidebar.offsetWidth
            : 0;
    }

    function resizeGlobe() {
        const sidebarWidth = getSidebarWidth();

        const width = window.innerWidth - sidebarWidth;
        const height = window.innerHeight - topOffset;

        container.style.left = `${sidebarWidth}px`;
        container.style.top = `${topOffset}px`;
        container.style.width = `${width}px`;
        container.style.height = `${height}px`;

        globe.width(width);
        globe.height(height);

        globe.pointOfView({
            lat: 0,
            lng: 0,
            altitude: 3.8  // This shows the whole globe clearly
        }, 0);
    }

    // Call once and attach listener
    resizeGlobe();
    window.addEventListener('resize', resizeGlobe);

    // Animate auto-rotate
    const controls = globe.controls();
    controls.autoRotate = true;
    controls.autoRotateSpeed = 0.3;

    let animationId;

    function animate() {
        animationId = requestAnimationFrame(animate);
        controls.update();
    }
    animate();

    // Return an object with a destroy method
    const globeObj = {
        globe,
        destroy() {
            window.removeEventListener('resize', resizeGlobe);
            if (animationId) cancelAnimationFrame(animationId);
            if (controls && controls.dispose) controls.dispose();
            if (container) container.innerHTML = '';

            animationId = null;
        }
    };

    globes[elementId] = globeObj;

    return globeObj;
}

export function createItineraryGlobe(elementId) {
    const container = document.getElementById(elementId);
    if (!container) {
        console.error(`Element with ID '${elementId}' not found`);
        return;
    }

    container.innerHTML = '';

    const globe = Globe()(container)
        .globeImageUrl('//cdn.jsdelivr.net/npm/three-globe/example/img/earth-blue-marble.jpg') // Day map
        .backgroundImageUrl('//unpkg.com/three-globe/example/img/night-sky.png')
        .backgroundColor('rgba(0,0,0,1)');

    function resizeGlobe() {
        const width = container.clientWidth;
        const height = container.clientHeight;

        globe.width(width);
        globe.height(height);
    }

    resizeGlobe();
    window.addEventListener('resize', resizeGlobe);

    const controls = globe.controls();
    controls.autoRotate = true;
    controls.autoRotateSpeed = 0.3;

    return {
        destroy() {
            window.removeEventListener('resize', resizeGlobe);
            // Dispose of globe if needed
            if (globe && globe.renderer) {
                globe.renderer.dispose();
            }
        }
    };
}

//function getColoredMarkerSvg(hexColor) {
//    return `
//<svg viewBox="-4 0 36 36" xmlns="http://www.w3.org/2000/svg">
//    <defs>
//        <linearGradient id="grad" x1="0%" y1="0%" x2="0%" y2="100%">
//            <stop offset="0%" stop-color="${hexColor}" stop-opacity="1"/>
//            <stop offset="100%" stop-color="${hexColor}" stop-opacity="0.7"/>
//        </linearGradient>
//        <filter id="shadow" x="-50%" y="-50%" width="200%" height="200%">
//            <feDropShadow dx="0" dy="1" stdDeviation="2" flood-color="rgba(0,0,0,0.4)"/>
//        </filter>
//    </defs>
//    <path fill="url(#grad)" stroke="white" stroke-width="1" filter="url(#shadow)"
//        d="M14,0 C21.732,0 28,5.641 28,12.6 C28,23.963 14,36 14,36 C14,36 0,24.064 0,12.6 C0,5.641 6.268,0 14,0 Z"></path>
//    <circle fill="black" cx="14" cy="14" r="7" stroke="white" stroke-width="1"></circle>
//</svg>
//`;
//}

const daysOfWeek = [
    "Sunday", "Monday", "Tuesday",
    "Wednesday", "Thursday", "Friday", "Saturday"
];

const dayColors = {
    Monday: "#FF4B4B",
    Tuesday: "#4CAF50",
    Wednesday: "#8E44AD",
    Thursday: "#FF8C42",
    Friday: "#4287F5",
    Saturday: "#FFD93D",
    Sunday: "#FF66B3"
};

export function createGlobe(elementId, places) {
    globeContainer = document.getElementById(elementId);
    if (!globeContainer) {
        console.error(`Element with ID '${elementId}' not found`);
        return;
    }

    const hasPlaces = places && places.length > 0;
    const fallbackPlaces = getDefaultPlaces();
    const hasFallback = fallbackPlaces && fallbackPlaces.length > 0;

    globeData = hasPlaces ? places : hasFallback ? fallbackPlaces : [];

    globeData.forEach(d => {
        const date = new Date(d.startDate); // convert string to Date
        const dayName = daysOfWeek[date.getDay()];
        d.id = d.markerId;
        d.lat = d.place?.lat;
        d.lng = d.place?.lng;
        d.size = 30;
        d.color = dayColors[dayName] || "#999999";;
    });

    globeInstance = Globe()(globeContainer)
        .globeTileEngineUrl((x, y, l) => `https://tile.openstreetmap.org/${l}/${x}/${y}.png`)
        .htmlElement(d => {
            try {
                if (!d || typeof d.lat !== 'number' || typeof d.lng !== 'number') {
                    console.warn('Invalid marker data:', d);
                    return null;
                }

                const el = document.createElement('div');
                el.style.color = d.color || 'red';
                el.style.width = `${d.size || 30}px`;
                el.style.pointerEvents = 'none';

                const wrapper = document.createElement('div');
                wrapper.className = 'marker-wrapper';
                wrapper.style.pointerEvents = 'auto';
                wrapper.style.display = 'inline-block';
                wrapper.style.width = `${d.size || 20}px`;
                wrapper.style.height = `${d.size || 20}px`;
                wrapper.style.color = d.iconColor || d.color || 'white';
                wrapper.style.cursor = 'pointer';
                wrapper.style.filter = 'drop-shadow(0 1px 2px rgba(0,0,0,.4))';
                wrapper.style.fontSize = `${d.size || 25}px`; 
                wrapper.style.transform = 'translate(-50%, -100%)';
                wrapper.style.position = 'absolute'; // ensures transform works relative to globe


                if (!markerSvg) {
                    console.error('markerSvg is missing!');
                    return null;
                }

                // USE d.html IF AVAILABLE, OTHERWISE FALLBACK
                const svgHtml = d.html || markerSvg;

                //console.log('markerSvg', markerSvg);
                wrapper.insertAdjacentHTML('beforeend', svgHtml);

                // Define named handlers so they can be removed later
                handleWheel = function (event) {
                    event.preventDefault();
                    const canvas = globeContainer.querySelector('canvas');
                    if (canvas) {
                        const newEvent = new WheelEvent(event.type, event);
                        canvas.dispatchEvent(newEvent);
                    }
                };

                handleClick = function (e) {
                    e.stopPropagation();
                    showMarkerPopup(wrapper, d); // `d` must also be available in this scope
                };

                wrapper.addEventListener('click', handleClick);

                wrapper.addEventListener('pointermove', (event) => {
                    const canvas = globeContainer.querySelector('canvas');
                    if (canvas) {
                        const newEvent = new PointerEvent(event.type, event);
                        canvas.dispatchEvent(newEvent);
                    }
                });

                el.appendChild(wrapper);
                return el;
            } catch (e) {
                console.error('Error creating marker element', e, d);
                return null;
            }
        })
        .htmlElementVisibilityModifier((el, isVisible) => {
            try {
                // Protect against null, undefined, or non-DOM element
                if (el && typeof el.style !== 'undefined') {
                    el.style.opacity = isVisible ? 1 : 0;
                } else {
                    console.warn('htmlElementVisibilityModifier received invalid element:', el);
                }
            } catch (e) {
                console.error('Error in visibility modifier:', e, el);
            }
        });

    //globeInstance.renderer().setClearColor(0xffffff);

    globeInstance._markerData = globeData;

    if (globeData.length > 0) {
        avgLat = globeData.reduce((sum, d) => sum + d.lat, 0) / globeData.length;
        avgLng = globeData.reduce((sum, d) => sum + d.lng, 0) / globeData.length;

        globeInstance.htmlElementsData(globeData);
    } else {
        // Default to center of US if no data
        avgLat = 39.8283;
        avgLng = -98.5795;
    }

    // Prepare coordinates
    const coordinates = (places || [])
        .map(p => ({
            lat: p.place?.lat ?? p.lat,
            lng: p.place?.lng ?? p.lng
        }))
        .filter(c => typeof c.lat === "number" && typeof c.lng === "number");

    clickHandler = function (e) {
        const popup = document.getElementById('markerPopup');
        if (!popup) return;

        if (!popup.contains(e.target) && !e.target.closest('.marker-wrapper')) {
            popup.style.display = 'none';
        }
    };
    document.addEventListener('click', clickHandler);

    // Fit globe once after creation
    fitResizeHandler = function () {
        fitGlobeToCoordinates(coordinates);
    };
    window.addEventListener('resize', fitResizeHandler);

    // Run the fit once on init
    fitGlobeToCoordinates(coordinates);

    return {
        destroy() {
            // Remove handlers
            if (fitResizeHandler) {
                window.removeEventListener("resize", fitResizeHandler);
                fitResizeHandler = null;
            }

            if (clickHandler) {
                document.removeEventListener("click", clickHandler);
                clickHandler = null;
            }

            if (globeContainer) globeContainer.innerHTML = "";

            // Nullify globals for GC
            globeInstance = null;
            globeContainer = null;
            currentDotNetHelper = null;
        }
    };
}

export function fitGlobeToCoordinates(coordinates) {
    if (!globeContainer || !globeInstance) {
        console.warn("Globe not initialized");
        return;
    }

    const rect = globeContainer.getBoundingClientRect();
    globeInstance.width(rect.width);
    globeInstance.height(rect.height);

    let targetLat = 39.8283; // Default US center
    let targetLng = -98.5795;
    let fitAltitude = 1.8;

    if (Array.isArray(coordinates) && coordinates.length > 0) {
        const lats = coordinates.map(c => c.lat);
        const lngs = coordinates.map(c => c.lng);

        const minLat = Math.min(...lats);
        const maxLat = Math.max(...lats);
        const minLng = Math.min(...lngs);
        const maxLng = Math.max(...lngs);

        const latSpan = maxLat - minLat;
        const lngSpan = maxLng - minLng;
        const maxSpanDeg = Math.max(latSpan, lngSpan);
        const paddedSpanDeg = maxSpanDeg * 1.4;

        fitAltitude = Math.tan((paddedSpanDeg * Math.PI) / 360) * 2.2;
        targetLat = (minLat + maxLat) / 2;
        targetLng = (minLng + maxLng) / 2;
    }

    setTimeout(() => {
        globeInstance.pointOfView(
            { lat: targetLat, lng: targetLng, altitude: fitAltitude },
            2500
        );
    }, 500);

    const controls = globeInstance.controls?.();
    if (controls) {
        controls.maxPolarAngle = Math.PI / 1.75;
        controls.minPolarAngle = 0.2;
    }
}

const markerSvg = `<svg viewBox="-4 0 36 36">
        <path fill="currentColor" d="M14,0 C21.732,0 28,5.641 28,12.6 C28,23.963 14,36 14,36 C14,36 0,24.064 0,12.6 C0,5.641 6.268,0 14,0 Z"></path>
        <circle fill="black" cx="14" cy="14" r="7"></circle>
    </svg>`;

export function addMarkerToGlobe(newMarker, iconType) {
    if (!globeInstance) {
        console.error('Globe instance is not initialized');
        return;
    }

    if (!globeInstance._markerData) {
        globeInstance._markerData = [];
    }

    const date = new Date(newMarker.startDate);
    const dayName = daysOfWeek[date.getDay()];
    newMarker.color = dayColors[dayName] || "#999999";


    // Add type-specific icon, e.g., for hotels
    if (iconType === "hotel") {
        newMarker.html = `<i class="bi bi-building-fill-gear"></i>`; // bootstrap hotel-style icon
        newMarker.iconColor = 'white'; // icon color
        newMarker.size = 25; // optional, controls icon size
    }

    globeInstance._markerData.push(newMarker);

    globeInstance.htmlElementsData([...globeInstance._markerData]);

    globeInstance.pointOfView(
        {
            lat: newMarker.lat,
            lng: newMarker.lng,
            altitude: 0.005
        },
        1000
    );
}

export function removeMarkerFromGlobe(markerId) {
    //if (!globeInstance) {
    //    console.error('Globe instance is not initialized');
    //    return;
    //}

    //if (!globeInstance._markerData) {
    //    console.warn('No marker data to remove');
    //    return;
    //}

    //// Remove the marker by some unique identifier (id, lat/lng, etc.)
    //globeInstance._markerData = globeInstance._markerData.filter(
    //    m => m.id !== markerId
    //);

    //// Re-render with updated data
    //globeInstance.htmlElementsData([...globeInstance._markerData]);
    if (!globeInstance || !globeInstance._markerData || !globeData) return;

    // Remove marker from _markerData
    globeInstance._markerData = globeInstance._markerData.filter(
        m => m.markerId !== markerId
    );

    // Also remove from globeData (used by fitGlobe)
    globeData = globeData.filter(d => d.markerId !== markerId);

    // Refresh HTML elements
    globeInstance.htmlElementsData([...globeInstance._markerData]);

    // Optionally refit globe bounds
    //fitGlobe();
}

function showMarkerPopup(markerElement, data) {
    const popup = document.getElementById('markerPopup');
    if (!popup) return;

    const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent);

    let mapLinksHtml = '';
    if (isIOS) {
        mapLinksHtml = `
            <button onclick="openMaps('${data.place.googlePlaceId}', ${data.lat}, ${data.lng}, 'apple')"
                    class="popup-map-btn apple-btn">
                Open in Apple Maps
            </button>
            <button onclick="openMaps('${data.place.googlePlaceId}', ${data.lat}, ${data.lng}, 'google')"
                    class="popup-map-btn google-btn">
                Open in Google Maps
            </button>
        `;
    } else {
        mapLinksHtml = `
            <button onclick="openMaps('${data.place.googlePlaceId}', ${data.lat}, ${data.lng}, 'google')"
                    class="popup-map-btn google-btn">
                Open in Google Maps
            </button>
        `;
    }

    popup.innerHTML = `
        <div style="display: flex; justify-content: space-between; align-items: center;">
            <strong><i class="bi bi-geo-alt-fill"></i> ${data.place.displayName}</strong>
            <button id="closePopupBtn" style="
                background: transparent;
                border: none;
                font-size: 18px;
                cursor: pointer;
                margin-left: 10px;
                line-height: 1;
            ">&times;</button>
        </div>
        <div style="margin-top: 8px; font-family: Arial, sans-serif; font-size: 1em; line-height:1.35; color:#333;">
            <div style="display: flex; align-items: center; gap: 6px; margin-bottom: 4px; font-weight: bold;">
                <i class="bi bi-clock"></i>
                <span>Time:</span>
            </div>
            <div style="margin-left: 22px; margin-bottom: 8px;">
                ${formatDateTime(data.startDate, data.startTime)}
            </div>

            <div style="display: flex; align-items: center; gap: 6px; margin-bottom: 4px; font-weight: bold;">
                <i class="bi bi-map"></i>
                <span>Address:</span>
            </div>
            <div style="margin-left: 22px; margin-bottom: 8px;">
                ${data.place.formattedAddress}
            </div>

             <div style="margin-left: 15px; margin-bottom: 10px;">
                ${mapLinksHtml}
            </div>
        </div>
    `;

    // Stronger styling for readability
    popup.style.pointerEvents = 'auto';
    popup.style.display = 'block';
    popup.style.position = 'absolute';
    popup.style.background = 'rgba(255,255,255,0.95)';
    popup.style.color = '#222';
    popup.style.fontWeight = '500';
    popup.style.padding = '8px 12px';
    popup.style.borderRadius = '8px';
    popup.style.boxShadow = '0 4px 12px rgba(0,0,0,0.15)';

    // Get marker position
    const rect = markerElement.getBoundingClientRect();

    // Temporarily show to measure size
    popup.style.left = '0';
    popup.style.top = '0';
    const popupWidth = popup.offsetWidth;
    const popupHeight = popup.offsetHeight;

    // Calculate horizontal position, clamped inside viewport
    let left = rect.left + window.scrollX + rect.width / 2 - popupWidth / 2;
    const maxLeft = window.innerWidth - popupWidth - 10;
    left = Math.min(Math.max(10, left), maxLeft);

    // Try showing above the marker
    let top = rect.top + window.scrollY - popupHeight - 5;

    // If popup goes above viewport, show below the marker instead
    if (top < window.scrollY + 10) {
        top = rect.bottom + window.scrollY + 5;
    }

    // Clamp bottom so it doesn't overflow off screen
    const maxTop = window.scrollY + window.innerHeight - popupHeight - 10;
    top = Math.min(Math.max(window.scrollY + 10, top), maxTop);

    // Apply final position
    popup.style.left = `${left}px`;
    popup.style.top = `${top}px`;

    // Close button
    document.getElementById("closePopupBtn")?.addEventListener("click", () => {
        popup.style.display = 'none';
    });
}

export function fitGlobe() {
    if (!globeContainer || !globeInstance) return;

    const rect = globeContainer.getBoundingClientRect();
    globeInstance.width(rect.width);
    globeInstance.height(rect.height);

    const hasData = globeData && globeData.length > 0;

    let targetLat = avgLat;
    let targetLng = avgLng;
    let fitAltitude;

    if (hasData) {
        // Map lat/lng from place object
        const lats = globeData.map(d => d.place?.lat).filter(lat => typeof lat === 'number');
        const lngs = globeData.map(d => d.place?.lng).filter(lng => typeof lng === 'number');

        if (lats.length === 0 || lngs.length === 0) {
            // Fallback if no valid coordinates
            targetLat = 39.8283;
            targetLng = -98.5795;
            fitAltitude = 1.8;
        } else {
            const minLat = Math.min(...lats);
            const maxLat = Math.max(...lats);
            const minLng = Math.min(...lngs);
            const maxLng = Math.max(...lngs);

            const latSpan = maxLat - minLat;
            const lngSpan = maxLng - minLng;
            const maxSpanDeg = Math.max(latSpan, lngSpan);
            const paddedSpanDeg = maxSpanDeg * 1.4;

            fitAltitude = Math.tan((paddedSpanDeg * Math.PI) / 360) * 2.2;

            // Center to average position
            targetLat = (minLat + maxLat) / 2;
            targetLng = (minLng + maxLng) / 2;
        }
    } else {
        // Default center on US if no marker data
        targetLat = 39.8283;
        targetLng = -98.5795;
        fitAltitude = 1.8; // Reasonable zoom-out to show the continent
    }

    setTimeout(() => {
        globeInstance.pointOfView({
            lat: targetLat,
            lng: targetLng,
            altitude: fitAltitude
        }, 2500);
    }, 500);

    const controls = globeInstance.controls();
    if (controls) {
        controls.maxPolarAngle = Math.PI / 1.75;
        controls.minPolarAngle = 0.2;
    }
}

function getDefaultPlaces() {
    return [
        //{ lat: 41.1579, lng: -8.6291, size: 30, color: 'blue', label: 'Porto' },
        //{ lat: 37.0194, lng: -7.9304, size: 30, color: 'green', label: 'Faro' },
        //{ lat: 38.7169, lng: -9.1399, size: 30, color: 'red', label: 'Lisbon' },
        //{ lat: 40.2111, lng: -8.4292, size: 30, color: 'orange', label: 'Coimbra' },
        //{ lat: 39.2362, lng: -8.6859, size: 30, color: 'purple', label: 'Santarém' },
        //{ lat: 41.6956, lng: -8.8345, size: 30, color: 'pink', label: 'Braga' },
        //{ lat: 38.5244, lng: -8.8882, size: 30, color: 'teal', label: 'Setúbal' },
        //{ lat: 39.8234, lng: -7.4932, size: 30, color: 'brown', label: 'Castelo Branco' },
        //{ lat: 38.0141, lng: -7.8671, size: 30, color: 'yellow', label: 'Beja' }
    ];
}

function formatDateTime(dateStr, timeStr) {
    // Combine into one ISO date-time string
    const dt = new Date(`${dateStr.split("T")[0]}T${timeStr}`);

    const datePart = dt.toLocaleDateString('en-US', {
        month: 'long',  // "July"
        day: 'numeric', // 31
        year: 'numeric' // 2025
    });

    const timePart = dt.toLocaleTimeString('en-US', {
        hour: 'numeric',
        minute: '2-digit',
        hour12: true    // AM/PM format
    }).toLowerCase(); // make "AM" -> "am"

    return `${datePart} @ ${timePart}`;
}

window.openMaps = function (placeId, lat, lng, target) {
    let url;
    if (target === 'apple') {
        url = `maps://?q=${lat},${lng}`; // Apple Maps
    } else {
        url = `https://www.google.com/maps/place/?q=place_id:${placeId}`; // Google Maps
    }
    window.open(url, '_blank');
};

function handleGlobalClick(e) {
    const popup = document.getElementById('markerPopup');
    const isInsidePopup = popup && popup.contains(e.target);
    const isMarker = e.target.closest('div')?.innerHTML?.includes('<path');

    if (!isInsidePopup && !isMarker) {
        hideMarkerPopup();
    }

    document.removeEventListener('click', handleGlobalClick);
}

export function scrollToElementById(id) {
    const element = document.getElementById(id);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

export function initAutocompleteElement(dotNetHelper) {
    currentDotNetHelper = dotNetHelper;
    setupAutocomplete();
}

export function recreateAutocomplete(dotNetHelper) {
    currentDotNetHelper = dotNetHelper;
    setupAutocomplete();
}

function setupAutocomplete() {
    const container = document.getElementById("autocompleteContainer");

    if (!container) {
        //console.error("❌ autocompleteContainer not found.");
        return;
    }


    container.innerHTML = ""; // Clear first
    // Create and insert the autocomplete element
    autocomplete = new google.maps.places.PlaceAutocompleteElement();
    //container.appendChild(autocomplete);

    //container.appendChild(autocomplete);

    //const selectedPlaceTitle = document.createElement('p');
    //selectedPlaceTitle.textContent = '';
    //container.appendChild(selectedPlaceTitle);
    //const selectedPlaceInfo = document.createElement('pre');
    //selectedPlaceInfo.textContent = '';
    //container.appendChild(selectedPlaceInfo);

    const wrapper = document.createElement("div");
    wrapper.style.position = "fixed";
    wrapper.style.top = "0";
    wrapper.style.left = "0";
    wrapper.style.right = "0";
    wrapper.style.zIndex = "1000";
    wrapper.appendChild(autocomplete);
    container.appendChild(wrapper);

    const selectedPlaceTitle = document.createElement('p');
    selectedPlaceTitle.textContent = '';
    container.appendChild(selectedPlaceTitle);
    const selectedPlaceInfo = document.createElement('pre');
    selectedPlaceInfo.textContent = '';
    container.appendChild(selectedPlaceInfo);
    selectedPlaceTitle.textContent = 'Selected Place:';

    //autocomplete.addEventListener('gmp-select', async ({ placePrediction }) => {
    //    const place = placePrediction.toPlace();
    //    await place.fetchFields({ fields: ['displayName', 'formattedAddress', 'location'] });
    //    selectedPlaceTitle.textContent = 'Selected Place:';
    //    selectedPlaceInfo.textContent = JSON.stringify(place.toJSON(), /* replacer */ null, /* space */ 2);
    //});

    autocompleteHandler = async ({ placePrediction }) => {
        await handlePlaceSelect(placePrediction);
    };

    //// Add event listener
    //autocomplete.addEventListener("gmp-select", handlePlaceSelect);
}

export function initAutocompleteElementById(dotNetHelper, elementId) {
    currentDotNetHelper = dotNetHelper;
    const container = document.getElementById(elementId);

    if (!container) {
        //console.error("❌ autocompleteContainer not found.");
        return;
    }

    container.innerHTML = ""; // Clear first
    // Create and insert the autocomplete element
    autocomplete = new google.maps.places.PlaceAutocompleteElement();
    container.appendChild(autocomplete);

    autocompleteHandler = async ({ placePrediction }) => {
        await handlePlaceSelect(placePrediction);
    };

    // Add event listener
    autocomplete.addEventListener("gmp-select", handlePlaceSelect);
}

async function handlePlaceSelect({ placePrediction }) {
    try {
        if (!currentDotNetHelper) {
            console.warn("⚠️ .NET helper not available, ignoring selection.");
            return;
        }

        const placeId = placePrediction?.placeId;
        if (!placeId) return;

        const alreadySaved = await currentDotNetHelper.invokeMethodAsync("IsPlaceSaved", placeId);

        let placeDto;

        if (alreadySaved) {
            placeDto = { googlePlaceId: placeId };
        } else {
            const place = placePrediction.toPlace();
            await place.fetchFields({
                fields: [
                    'id',
                    'displayName',
                    'formattedAddress',
                    'addressComponents',
                    'location',
                    'openingHours',
                    'photos',
                    'postalAddress',
                    'rating',
                    'userRatingCount',
                    'primaryType',
                    'primaryTypeDisplayName',
                    'types'
                ]
            });

            const countryComponent = place.addressComponents.find(c => c.types.includes("country"));
            const countryCode = countryComponent?.shortText; // e.g. "US", "FR"

            placeDto = JSON.parse(JSON.stringify(place));
            placeDto.googlePlaceId = place.id;
            delete placeDto.id;

        }

        if (currentDotNetHelper) {
            await currentDotNetHelper.invokeMethodAsync("OnPlaceChanged", placeDto);
        } else {
            console.warn("⚠️ .NET helper not available.");
        }

    } catch (err) {
        console.error("❌ Error handling place selection:", err);
    }
}

export function updateBottomSheetHeight(height) {
    const sheet = document.getElementById("bottomSheet");
    if (sheet) {
        sheet.style.height = height;
        sheet.style.maxHeight = height;
    }
}

export function scrollButtonRow(containerId, direction) {
    const container = document.getElementById(containerId);
    if (!container) return;

    const scrollAmount = 150;
    container.scrollBy({
        left: direction * scrollAmount,
        behavior: 'smooth'
    });
}

export function getScrollInfo(elementId) {
    const el = document.getElementById(elementId);
    if (!el) return null;

    return {
        scrollLeft: Math.round(el.scrollLeft),
        scrollWidth: el.scrollWidth,
        clientWidth: el.clientWidth
    };
}

export function registerResizeHandler(dotNetHelper) {
    window.addEventListener("resize", () => {
        dotNetHelper.invokeMethodAsync("CheckScrollButtons");
    });
}

export function getScrollState(containerId) {
    const el = document.getElementById(containerId);
    if (!el) return [true, true];
    const isAtStart = el.scrollLeft <= 0;
    const isAtEnd = el.scrollLeft + el.clientWidth >= el.scrollWidth - 1;
    return [isAtStart, isAtEnd];
}

export function initOffcanvasOutsideClick(offcanvasId, modalId, dotnetHelper) {

    const offcanvasEl = document.getElementById(offcanvasId);

    document.addEventListener('click', function (event) {

        const modalEl = document.getElementById(modalId); // 🟢 move this inside the handler

        const isOpen = offcanvasEl.classList.contains('show');
        const clickedInsideOffcanvas = offcanvasEl.contains(event.target);
        const clickedInsideModal = modalEl ? modalEl.contains(event.target) : false;

        if (isOpen && !clickedInsideOffcanvas && !clickedInsideModal) {
            closeOffcanvas(offcanvasId);
            //// uncomment this if want to prevent close
            //dotnetHelper.invokeMethodAsync('HandleOffcanvasOutsideClick');
        }
    });
}

export function openOffcanvasHalfScreen(offcanvasId) {
    const el = document.getElementById(offcanvasId);
    if (!el) return;

    let instance = bootstrap.Offcanvas.getInstance(el);
    if (!instance) {
        instance = new bootstrap.Offcanvas(el);
    }
    instance.show();
}

export function toggleOffcanvasFullScreen(offcanvasId, isFullScreen) {
    const el = document.getElementById(offcanvasId);
    if (!el) return;

    if (isFullScreen) {
        el.classList.add('fullscreen');
    } else {
        el.classList.remove('fullscreen');
    }
};

export function closeOffcanvas(offcanvasId) {
    const el = document.getElementById(offcanvasId);
    if (!el) return;

    const instance = bootstrap.Offcanvas.getInstance(el);
    if (instance) {
        instance.hide();

    }
}

export function blurActiveElement() {
    if (document.activeElement) {
        document.activeElement.blur();
    }
}

export function registerOffcanvasCloseHandler(offcanvasId, dotNetHelper) {
    const offcanvasEl = document.getElementById(offcanvasId);
    if (!offcanvasEl) {
        console.error("Offcanvas not found:", offcanvasId);
        return;
    }

    if (offcanvasListeners.has(offcanvasId)) return;

    const handler = () => {
        //console.log("Offcanvas closed — recreating autocomplete");
        recreateAutocomplete(dotNetHelper);

        if (dotNetHelper && offcanvasId == "offcanvas") {
            dotNetHelper.invokeMethodAsync("OnOffcanvasClosed")
                .catch(err => console.error("Error calling .NET method:", err));
        }
    };

    offcanvasEl.addEventListener("hidden.bs.offcanvas", handler);
    offcanvasListeners.add(offcanvasId);
}

export function removeAllListeners() {


    if (clickHandler) {
        document.removeEventListener('click', clickHandler);
        clickHandler = null;
    }
    if (fitResizeHandler) {
        window.removeEventListener('resize', fitResizeHandler);
        fitResizeHandler = null;
    }

    if (autocomplete) {
        if (autocompleteHandler) {
            autocomplete.removeEventListener("gmp-select", autocompleteHandler);
            autocompleteHandler = null;
        }
        autocomplete = null;
    }

    if (wrapper) {
        if (handleWheel) wrapper.removeEventListener('wheel', handleWheel);
        if (handleClick) wrapper.removeEventListener('click', handleClick);
        handleWheel = null;
        handleClick = null;
        wrapper = null;
    }

    const container = document.getElementById("autocompleteContainer");
    if (container) {
        container.innerHTML = "";
    }

    // Remove popups
    const popup = document.getElementById("markerPopup");
    if (popup) popup.style.display = "none";

    document.body.classList.remove("modal-open");
    document.body.style.overflow = "auto";

    document.querySelectorAll(".modal-backdrop, .offcanvas-backdrop").forEach((b) => b.remove());

    const offcanvasEl = document.getElementById("offcanvas");
    if (offcanvasEl) {
        const bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvasEl);
        if (bsOffcanvas) {
            bsOffcanvas.hide();
        }

        // Remove listener if previously added
        if (offcanvasListeners.has(offcanvasId)) {
            offcanvasEl.replaceWith(offcanvasEl.cloneNode(true)); // This removes all listeners
            offcanvasListeners.delete(offcanvasId);
        }
    }

    // Add cleanup of the .NET reference
    if (currentDotNetHelper) {
        try {
            currentDotNetHelper.dispose();
        } catch (e) {
            console.warn("DotNetObjectReference already disposed", e);
        }
        currentDotNetHelper = null;
    }
}

export function registerOutsideClickHandler(offcanvasId, dotNetHelper) {
    document.addEventListener('click', function (e) {
        // 1. Ignore clicks inside *any* offcanvas

        if (e.target.closest('.menu-dropdown')) {
            //dotNetHelper.invokeMethodAsync('OnClickOutside', offcanvasId);
            return
        }

        // 2. Ignore clicks inside dropdowns or other exempt elements
        if (e.target.closest('.dropdown-wrapper')) {
            dotNetHelper.invokeMethodAsync('ClickedMenuButton');
            return;
        }

        if (e.target.closest('.offcanvas') &&
            (offcanvasId == "offcanvasItinerarySchedule") ||
            (offcanvasId == "offcanvasAccomodationSchedule")) {
            dotNetHelper.invokeMethodAsync('CheckIfClickedOutside', offcanvasId);
            return;
        }



        if (offcanvasId == 'offcanvas') {
            // 3. If clicked outside of all offcanvases, call .NET method
            dotNetHelper.invokeMethodAsync('OnClickOutside', offcanvasId);
        }
    });
}

export function isIPhone() {
    return /iPad|iPhone|iPod/.test(navigator.userAgent);
}

clickHandler = function (e) {
    if (!globeContainer || !globeInstance) return; // <-- add this guard
    const popup = document.getElementById('markerPopup');
    if (!popup) return;

    if (!popup.contains(e.target) && !e.target.closest('.marker-wrapper')) {
        popup.style.display = 'none';
    }
};

window.initAllSortableLists = () => {
    const lists = document.querySelectorAll('.sortable-container');
    lists.forEach(el => {
        Sortable.create(el, {
            group: 'PlacesGroup',   // make sure group matches your Blazor group
            handle: '.bb-sortable-list-handle',
            animation: 150,
            scroll: true,           // auto-scroll
            scrollSensitivity: 60,
            scrollSpeed: 10,
            fallbackOnBody: true,   // needed for mobile dragging
            touchStartThreshold: 5
        });
    });
};
export function DestroyItineraryGlobe() {
    if (globeInstance) {
        try {
            globeInstance.destroy(); // if using some 3D library
        } catch { }
        globeInstance = null;
    }
    globeContainer = null;
}

export function copyTextToClipboard(text) {
    if (!text) return;
    console.log("JS function called with:", text);
    navigator.clipboard.writeText(text)
        .then(() => console.log("Copied to clipboard"))
        .catch(err => console.error("Failed to copy: ", err));
}