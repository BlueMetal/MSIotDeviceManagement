//Style
import './css/site.scss';

//Assets
//require.context('./assets/images', true, /^\.\//);

//Html
require.context('./app', true, /.html/);

//Javascript
import './app/app';
import './app/directives';
import './app/filters';
import './app/controllers';
import './app/services';