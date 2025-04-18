import { UserManager } from 'oidc-client';


const oidcSettings = {
    authority: 'https://localhost:5443',
    redirect_uri: 'https://localhost:3000/signin-oidc',
    response_type: 'code',
    client_id: 'bff',
    scope: 'openid profile userApi',
    post_logout_redirect_uri: 'https://localhost:3000/',
    storeSigninState: true,
};

const userManager = new UserManager(oidcSettings);

export default userManager;
