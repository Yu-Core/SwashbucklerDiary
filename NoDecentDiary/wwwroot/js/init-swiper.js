function swiperInit(dotNetCallbackRef, callbackMethod,id) {
    console.log('Entered initSwiper!');
    let className = "." + id;
    window[id] = new Swiper(className, {
        observer: true,
        observeParents: true,
        observeSlideChildren: true,
        autoHeight: true,
        on: {
            slideChangeTransitionEnd: function () {
                dotNetCallbackRef.invokeMethodAsync(callbackMethod, this.activeIndex);
                //alert(this.activeIndex);//切换结束时，告诉我现在是第几个slide
            },
        }
    });
}

function changeSwiperIndex(index) {
    swiper.slideTo(index);
}

function initSwiperForecastHours() {
    window.forecastHoursSwiper = new Swiper('.swiper-forecast-hours', {
        slidesPerView: 'auto',
        touchAngle: 90,
        freeMode: {
            enabled: true,  
           /* momentumRatio: 2,*/
            momentumVelocityRatio: 2,
        },
    });
}

function updateSwiper() {
    window.swiper.update();
}
