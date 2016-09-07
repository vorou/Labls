module.exports = {
    files: {
        javascripts: {
            joinTo: {
                'vendor.js': /^(?!app)/,
                'app.js': /^app/
            }
        },
        stylesheets: {
            joinTo: 'app.css'
        }
    },

    plugins: {
        babel: {
            presets: ['es2015', 'react']
        }
    },

    npm: {
        enabled: true,
        whitelist: ["jquery"],
        globals: {
            $: 'jquery',
            jQuery: 'jquery'
        }
    }
};
