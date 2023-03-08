module.exports = function(md, options) {
	md.core.ruler.push('h_attr', function(state) {
		var tokens = state.tokens;
		// console.log(tokens)
		tokens.forEach(function(token, i) {
			if (token.type == 'heading_open') {
				var content = token.conent
				var attr = getAttr(tokens[i+1])
				if(attr)
					setAttr(token, attr.name,attr.value)
			}
		})
	})

	//兼容
	const originalHeadingOpen = md.renderer.rules.heading_open ||
		function(...args) {
			const [tokens, idx, options, , self] = args
			return self.renderToken(tokens, idx, options)
		}

	md.renderer.rules.heading_open = function(...args) {
		console.log('hi');
		args[0].forEach(function(token, i) {
			if (token.type == 'heading_open') {
				var reg = /(\w+\s){(.*?)}/g
				args[0][i + 1].children[0].content = args[0][i + 1].children[0].content.replace(reg,'$1').trim()
			}
		})
		return originalHeadingOpen.apply(this, args)
	}

	function setAttr(token, name, value) {
		var index = token.attrIndex(name)
		var _value = token.attrGet(name)

		if (index === -1) {
			token.attrPush([name, value])
		} else if (!_value) {
			token.attrJoin(name, value)
		}
	}

	function getAttr(token,onlyV) {
		var content = token.content
		var reg = /\w+\s{(.*?)}/g
		var selector = content.replace(reg, '$1')
		var type = selector.substr(0, 1)

		var attr_v = selector.substr(1)
		if(onlyV) return attr_v
		var attr_name 
		switch (type) {
			case '.':
				attr_name = 'class'
				break
			case '#':
				attr_name = 'id'
				break
			default :
				break
		}
		if(attr_name){
			return {
				name: attr_name,
				value: attr_v
			}
		}
	}
}