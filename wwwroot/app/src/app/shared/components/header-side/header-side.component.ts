import { Component, OnInit, EventEmitter, Input, ViewChildren, Output, Renderer2 } from '@angular/core';
import { ThemeService } from '../../services/theme.service';
import { LayoutService } from '../../services/layout.service';
import { TranslateService } from '@ngx-translate/core';
import { JwtAuthService } from '../../services/auth/jwt-auth.service';
import { EgretNotifications2Component } from '../egret-notifications2/egret-notifications2.component';
import { UserService } from '../../../views/System/User/user.service';

@Component({
    selector: 'app-header-side',
    templateUrl: './header-side.template.html'
})
export class HeaderSideComponent implements OnInit {
    @Input() notificPanel;
    @ViewChildren(EgretNotifications2Component) noti;
    public availableLangs = [{
        name: 'EN',
        code: 'en',
        flag: 'flag-icon-us'
    },
    {
        name: 'TR',
        code: 'tr',
        flag: 'flag-icon-tr'
    },
    {
        name: 'AR',
        code: 'ar',
        flag: 'flag-icon-sa'
    },
    ];
    currentLang: any;

    public egretThemes;
    public layoutConf: any;
    userInfo: any;
    constructor(
        private themeService: ThemeService,
        private layout: LayoutService,
        public translate: TranslateService,
        private renderer: Renderer2,
        public jwtAuth: JwtAuthService,
        private userService: UserService,
    ) {
        this.userInfo = JSON.parse(localStorage.getItem("USER_INFO"));
        if (!this.userInfo.languageCode)
            this.currentLang = this.availableLangs[1];
        else this.currentLang = this.availableLangs.find(x => x.code == this.userInfo.languageCode)
        this.currentLang.code == 'ar' ? document.body.setAttribute('dir', 'rtl') : document.body.setAttribute('dir', 'ltr');
    }
    ngOnInit() {
        this.egretThemes = this.themeService.egretThemes;
        this.layoutConf = this.layout.layoutConf;
        this.translate.use(this.currentLang.code);
    }
    setLang(lng) {

        this.currentLang = lng;

        this.currentLang.code == 'ar' ? document.body.setAttribute('dir', 'rtl') : document.body.setAttribute('dir', 'ltr');
         

        this.translate.use(lng.code);
        this.userService.setUserLanguage(lng.code).subscribe(function () {
            this.userInfo = JSON.parse(localStorage.getItem("USER_INFO"));
            this.userInfo.languageCode = lng.code;
            localStorage.setItem("USER_INFO", JSON.stringify(this.userInfo));
        });

    }
    changeTheme(theme) {
        // this.themeService.changeTheme(theme);
    }
    toggleNotific() {
        this.notificPanel.toggle();
    }
    toggleSidenav() {
        if (this.layoutConf.sidebarStyle === 'closed') {
            return this.layout.publishLayoutChange({
                sidebarStyle: 'full'
            });
        }
        this.layout.publishLayoutChange({
            sidebarStyle: 'closed'
        });
    }

    toggleCollapse() {
        // compact --> full
        if (this.layoutConf.sidebarStyle === 'compact') {
            return this.layout.publishLayoutChange({
                sidebarStyle: 'full',
                sidebarCompactToggle: false
            }, { transitionClass: true });
        }

        // * --> compact
        this.layout.publishLayoutChange({
            sidebarStyle: 'compact',
            sidebarCompactToggle: true
        }, { transitionClass: true });

    }

    onSearch(e) {
        //   console.log(e)
    }
}
