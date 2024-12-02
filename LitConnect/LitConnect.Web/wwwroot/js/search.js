function initializeSearch(searchInputId, containerId) {
    const searchInput = document.getElementById(searchInputId);
    if (!searchInput) return;

    searchInput.addEventListener('input', function (e) {
        const searchText = e.target.value.toLowerCase();

        // Special handling for members table
        if (containerId === 'membersTable') {
            const rows = document.querySelectorAll('#membersTable tr');
            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(searchText) ? '' : 'none';
            });
            return;
        }

        // Regular container search
        const items = document.querySelectorAll('.' + containerId);
        items.forEach(item => {
            const text = item.textContent.toLowerCase();
            item.style.display = text.includes(searchText) ? '' : 'none';
        });
    });
}

// Initialize all search boxes when the document is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Members search
    initializeSearch('memberSearch', 'membersTable');

    // BookClub Details page searches
    initializeSearch('booksSearch', 'book-container');
    initializeSearch('meetingsSearch', 'meeting-container');
    initializeSearch('discussionsSearch', 'discussion-container');

    // BookClub Index page search
    initializeSearch('bookClubSearch', 'bookclub-container');

    // Book Index page search
    initializeSearch('bookSearch', 'book-container');

    // ReadingList Index page search
    initializeSearch('readingListSearch', 'readinglist-container');
});