export const config = {
    apiUrl: 'http://localhost:39210/api',
  
    loginPage:'/sessions/signin',
    authRoles: {
        sa: ['SA'], // Only Super Admin has access
        admin: ['SA', 'Admin'], // Only SA & Admin has access
        editor: ['SA', 'Admin', 'Editor'], // Only SA & Admin & Editor has access
        user: ['SA', 'Admin', 'Editor', 'User'], // Only SA & Admin & Editor & User has access
        guest: ['SA', 'Admin', 'Editor', 'User', 'Guest'] // Everyone has access
    }
}