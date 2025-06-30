import React, { RefObject, useState } from 'react';
import {
    ActionMenu,
    ActionMenuHeadline,
    BarItemDraggable,
    BarItemGroup,
    Button,
    ButtonColor,
    ButtonSize,
    Card,
    Cols,
    Column,
    DropDownSelectMenu,
    MenuItem,
    Row,
    SelectMenu,
    Spacing,
    Stack,
    TextArea
} from '@kentico/xperience-admin-components';
import { usePageCommand, usePageCommandProvider } from '@kentico/xperience-admin-base';

/** Copy of C# DatabaseTable class */
interface DatabaseTable {
    name: string;
    columns: string[];
}

/** Copy of C# SavedQuery class */
interface SavedQuery {
    id: number;
    name: string;
    text: string;
    order: number;
}

/** Copy of C# EditSqlTemplateClientProperties class */
interface EditQueryClientProperties {
    tables: DatabaseTable[];
    query: string | undefined;
    savedQueries: SavedQuery[];
}

export const EditQueryTemplate = (props: EditQueryClientProperties) => {
    const { executeCommand } = usePageCommandProvider();
    const textAreaRef = React.createRef<HTMLTextAreaElement>();
    const [queryText, setQueryText] = useState(props.query);
    const [savedQueries, setSavedQueries] = useState(props.savedQueries);
    const { execute: runSql } = usePageCommand<void, string>("RunSql");
    const { execute: notify } = usePageCommand<void, string>("Notify");
    /**
     * Save handler accepts a SavedQuery with only text and name. Returned object contains all properties of the database record.
     */
    const { execute: saveQuery } = usePageCommand<SavedQuery, SavedQuery>("SaveQuery", {
        after: newQuery => {
            if (!newQuery) {
                return;
            }

            const newQueries = [...savedQueries];
            newQueries.push(newQuery);
            setSavedQueries(newQueries);
        }
    });
    /**
     * Delete handler accepts the ID to delete, and returns the ID of the deleted record or zero.
     */
    const { execute: deleteQuery } = usePageCommand<number, number>("DeleteQuery", {
        after: async id => {
            if (!id || id <= 0) {
                return;
            }

            const newQueries = savedQueries.filter(q => q.id !== id);

            // Sorting can bug if there is missing index in ordering- update orders in UI and backend
            newQueries.forEach((query, i) => query.order = i);
            setSavedQueries(newQueries);
            await executeCommand<boolean, SavedQuery[]>("UpdateSavedOrder", newQueries);
        }
    });

    /**
     * Click handler to delete a saved query.
     */
    const deleteClick = (id: number) => {
        var confirmed = confirm('Are you sure you want to delete this query?');
        if (!confirmed) {
            return;
        }

        deleteQuery(id);
    };

    /**
     * Click handler to transfer the text of a saved query to the text box.
     */
    const transferClick = (text: string) => {
        if (!textAreaRef.current) {
            return;
        }

        setQueryText(text);
        textAreaRef.current.textContent = text;
        notify('Query copied to editor');
    };

    /**
     * Click handler for table name in listing to generate SELECT query.
     */
    const generateQuery = (table: DatabaseTable) => {
        if (!textAreaRef.current) {
            return;
        }

        const q = `SELECT ${table.columns.join(', ')} FROM ${table.name}`;
        setQueryText(q);
        textAreaRef.current.textContent = q;
        notify('SQL query generated');
    };

    /**
     * Click handler for clearing the editor text.
     */
    const clearClick = () => {
        if (!textAreaRef.current) {
            return;
        }

        setQueryText('');
        textAreaRef.current.textContent = '';
    };

    /**
     * Click handler to execute the current text.
     */
    const runClick = () => {
        if (!queryText || queryText.length <= 0) {
            alert('Please enter a query to execute.');

            return;
        }

        runSql(queryText);
    };

    /**
     * Click handler for copying the current text to clipboard.
     */
    const copyClick = async () => {
        if (!queryText || queryText.length <= 0) {
            return;
        }

        await navigator.clipboard.writeText(queryText);
        notify('Query copied to clipboard');
    };

    /**
     * Click handler for saving the current text as new SQL query.
     */
    const saveClick = async () => {
        if (!queryText || queryText.length <= 0) {
            alert('Please enter a query in the editor');

            return;
        }

        const name = prompt('Enter new query name');
        if (!name) {
            return;
        }

        saveQuery({ id: 0, order: 0, name: name, text: queryText });
    };

    /**
     * Handler after re-ordering the saved queries bar items.
     */
    const savedQueryDragEnd = async (dropResult: any) => {
        if (dropResult.source.index === dropResult.destination?.index) {
            return;
        }

        const sourceIndex = dropResult.source.index;
        const destinationIndex = dropResult.destination?.index || 0;
        const newQueries = updateQueryOrder(savedQueries, sourceIndex, destinationIndex);
        const updateSuccessful = await executeCommand<boolean, SavedQuery[]>("UpdateSavedOrder", newQueries);

        if (!updateSuccessful) {
            updateQueryOrder(newQueries, destinationIndex, sourceIndex);
        }
    };


    const updateQueryOrder = (oldQueries: SavedQuery[], sourceIndex: number, destinationIndex: number) => {
        const newQueries = [...oldQueries];
        const [sourceField] = newQueries.splice(sourceIndex, 1);
        newQueries.splice(destinationIndex, 0, sourceField);
        newQueries.forEach((query, i) => query.order = i);
        setSavedQueries(newQueries);

        return newQueries;
    };

    const renderSavedQueries = () =>
        savedQueries.map(q =>
            <BarItemDraggable
                key={q.name}
                index={q.order}
                draggableId={q.id.toString()}
                leadingButtons={[
                    {
                        label: 'Run',
                        icon: 'xp-caret-right',
                        tooltip: 'Execute query',
                        onClick: () => runSql(q.text)
                    },
                    {
                        label: 'Copy',
                        icon: 'xp-doc-copy',
                        tooltip: 'Copy query to text box',
                        onClick: () => transferClick(q.text)
                    },
                    {
                        label: 'Delete',
                        icon: 'xp-bin',
                        tooltip: 'Delete',
                        onClick: () => deleteClick(q.id)
                    }
                ]}
                headerColumns={[
                    {
                        content: <span>{q.name}</span>
                    }
                ]}>
                <span>{q.text}</span>
            </BarItemDraggable>
        );

    const renderTextActions = () =>
        <>
            <Button label='Save' color={ButtonColor.Primary} size={ButtonSize.S} onClick={saveClick} icon='xp-doc-plus' />
            <Button label='Copy' color={ButtonColor.Tertiary} size={ButtonSize.S} onClick={copyClick} icon='xp-doc-copy' />
            <Button label='Clear' color={ButtonColor.Tertiary} size={ButtonSize.S} onClick={clearClick} icon='xp-doc-torn' />
            {props.tables.length > 0 &&
                <DropDownSelectMenu renderTrigger={(ref, onTriggerClick) => (
                    <Button
                        label='Tables'
                        size={ButtonSize.S}
                        borderless
                        icon='xp-database'
                        buttonRef={ref as RefObject<HTMLButtonElement>}
                        onClick={() => onTriggerClick()} />
                )}>
                    {
                        props.tables.map(table =>
                            <MenuItem primaryLabel={table.name} onClick={() => generateQuery(table)} />
                        )
                    }
                </DropDownSelectMenu>
            }
        </>

    return (
        <Row spacing={Spacing.XL}>
            <Column cols={Cols.Col1} />
            <Column cols={Cols.Col10}>
                <Stack spacing={Spacing.XL}>
                    <Button label='Run' icon='xp-caret-right' color={ButtonColor.Primary} onClick={runClick} />
                    <Card headline='Query'>
                        <TextArea
                            minRows={10}
                            maxRows={40}
                            value={queryText}
                            textAreaRef={textAreaRef}
                            placeholder='Enter SQL query...'
                            onChange={(e) => setQueryText(e.target.value)}
                            renderActions={renderTextActions} />
                    </Card>
                    {props.savedQueries.length > 0 &&
                        <Card headline='Saved queries'>
                            <BarItemGroup droppableId='savedQueryDroppable' onDragEnd={savedQueryDragEnd}>
                                {renderSavedQueries()}
                            </BarItemGroup>
                        </Card>
                    }
                </Stack>
            </Column>
            <Column cols={Cols.Col1} />
        </Row>
    );
};
