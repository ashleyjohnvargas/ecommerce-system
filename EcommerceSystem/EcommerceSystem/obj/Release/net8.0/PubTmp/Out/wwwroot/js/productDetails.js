function scrollImages(button, direction) {
    const scrollableContainer = button.parentElement.querySelector('.scrollable-images');
    const scrollAmount = scrollableContainer.offsetWidth; // Scroll by the width of the container
    scrollableContainer.scrollBy({
        left: direction * scrollAmount,
        behavior: 'smooth'
    });
}

function navigatePage(page) {
    // Navigate to the new page URL with the correct page number
    window.location.href = '/Product/Details?page=' + page;
}
