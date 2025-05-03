
// 3. Compteur de chiffres sur les cartes
if (typeof $ !== 'undefined' && $.fn.counterUp) {
    $('.cardNumber').counterUp({
        delay: 10,
        time: 1200,
    });
}
