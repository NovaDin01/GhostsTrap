mergeInto(LibraryManager.library, {
  // Keep explicit signature/deps for IL2CPP/WebGL compatibility.
  ShowYandexFullscreenAd__sig: 'vi',
  ShowYandexFullscreenAd__deps: ['$UTF8ToString'],
  ShowYandexFullscreenAd: function (placementPtr) {
    // Decode argument to preserve expected call shape from previous builds.
    var placement = '';
    if (placementPtr) {
      placement = UTF8ToString(placementPtr);
    }

    // No-op by design: Yandex ads are removed.
    // Keep optional hook for legacy template JS to avoid null access errors.
    if (typeof window !== 'undefined' && typeof window.Yandex_ShowFullscreenAd === 'function') {
      try {
        window.Yandex_ShowFullscreenAd(placement);
      } catch (e) {
        // swallow - gameplay must continue even if host scripts fail
      }
    }
  }
});
