/*! 使用node.js管理前端项目 2015-08-17 by-wangyafei*/
module.exports = function (grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        //filerev: {
        //    dist: {
        //        src: [
        //          'images/{,*/}*.{png,jpg,jpeg,gif,webp,svg}'
        //        ]
        //    }
        //},
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> by-wangyafei*/\n'
            },
            my_target: {
                files: [{
                    expand: true,
                    src: 'widget/**/*.js',
                    dest: ''
                }, {
                    expand: true,
                    src: 'ad/**/*.js',
                    dest: ''
                }, {
                    expand: true,
                    src: 'bin/**/*.js',
                    dest: ''
                }, {
                    expand: true,
                    src: 'control/**/*.js',
                    dest: ''
                }, {
                    expand: true,
                    src: 'js/**/*.js',
                    dest: ''
                }, {
                    expand: true,
                    src: 'jsapi/**/*.js',
                    dest: ''
                }, {
                    expand: true,
                    src: 'pages/**/*.js',
                    dest: ''
                }]
            }
        },
        
        autoprefixer: {
            target: {
                files: [{
                    expand: true,
                    src: '**/*.css',
                    dest: ''
                }]
            }
        },

        cssmin: {
            target: {
                files: [{
                    expand: true,
                    src: '**/*.css',
                    dest: ''
                }]
            }
        },
        
        htmlmin: {
            dist: {
                options: {
                    removeComments: true,
                    collapseWhitespace: true,
                    minifyJS: true,
                    minifyCSS: true,
                    removeComments: true
                    //removeCommentsFromCDATA: true,
                    //collapseBooleanAttributes: true,
                    //removeAttributeQuotes: true,
                    //removeRedundantAttributes: true,
                    //useShortDoctype: true,
                    //removeEmptyAttributes: true,
                    //removeOptionalTags: true
                },
                files: [{
                    expand: true,
                    src: '**/*.html',
                    dest: ''
                }]
                //files: {
                //    //'Login.aspx': 'Login.aspx',
                //    //'Default.aspx':'Default.aspx',
                //    'street/street-foot.html': 'street/street-foot.html',
                //    'street/street-map.html': 'street/street-map.html',
                //    'street/street-return.html': 'street/street-return.html',
                //    //'pages/Thematic.aspx':'pages/Thematic.aspx',
                //    //'pages/ResourceService.aspx':'pages/ResourceService.aspx',
                //    //'pages/Metadata.aspx':'pages/Metadata.aspx',
                //    //'pages/HistoryContrast.aspx':'pages/HistoryContrast.aspx',
                //    //'pages/API.aspx':'pages/API.aspx',
                //    'jsapi/index.html': 'jsapi/index.html',
                //    'jsapi/map.html': 'jsapi/map.html',
                //    'control/map.html': 'control/map.html',
                //    'ad/index.html': 'ad/index.html'
                //}
            }
        }
    });
    /*
	static:{
		options:{
			optimizationLevel:3,
			svgoPlugins:[{removeViewBox:false}],
			use:[mozjpeg()]
		},
		files:{
			
		},
	},
		imagemin:{
			dynamic:{
				files:[{
					expand:true,
					src:['**\/\*.{png,jpg,gif}'],
					dest:''
				}]
			}
		}
	*/
    
    //加载静态资源版本化插件
    //grunt.loadNpmTasks('grunt-filerev');
    // 加载包含JS文件压缩的插件。
    grunt.loadNpmTasks('grunt-contrib-uglify');
    //grunt.loadNpmTasks('grunt-autoprefixer');
    // 加载包含CSS压缩的任务插件
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    // 加载HTML和aspx文件压缩的任务插件
    grunt.loadNpmTasks('grunt-contrib-htmlmin');
    // 加载图片文件压缩的任务插件
    //grunt.loadNpmTasks('grunt-contrib-imagemin');
    // 默认被执行的任务列表。
    grunt.registerTask('default', ['uglify', 'cssmin', 'htmlmin']);
};