mergeInto(LibraryManager.library, {
  ShowYandexFullscreenAd: function (placementPtr) {
    var placement = UTF8ToString(placementPtr);

    // Вызываем функцию, объявленную в index.html
    if (typeof window !== "undefined" && typeof window.Yandex_ShowFullscreenAd === "function") {
      window.Yandex_ShowFullscreenAd(placement);
    } else {
      console.log("Yandex_ShowFullscreenAd is not defined yet. Placement:", placement);
    }
  }
});

