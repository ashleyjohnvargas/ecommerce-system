function scrollImages(button, direction) {
    const slider = button.parentElement.querySelector('.scrollable-images');
    const scrollAmount = direction * 200; // Adjust scroll distance (in pixels)
    slider.scrollBy({ left: scrollAmount, behavior: 'smooth' });
}

function navigatePage(page) {
    // Navigate to the new page URL with the correct page number
    window.location.href = '/Product/Details?page=' + page;
}
