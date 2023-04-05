if FORMAT:match 'latex' then
    function Header(elem)
        if (elem.level == 1) then
            return {
                pandoc.RawInline('latex', '\\chapwithtoc{' .. pandoc.utils.stringify(elem.content) .. '}\n')
            }
        end
        return elem
    end
end